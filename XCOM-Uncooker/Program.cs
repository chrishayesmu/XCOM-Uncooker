using Microsoft.Extensions.Logging;
using System.IO;
using UnrealArchiveLibrary.Unreal;
using UnrealPackageLibrary;

namespace XCOM_Uncooker
{
    internal class Program
    {
        private static readonly Logger Log = new Logger("XCOM Uncooker");

        // Temp directory which will contain uncompressed versions of compressed archives, if needed.
        // The main Program.cs is responsible for deleting this when the process exits.
        public static readonly DirectoryInfo TempDirectory = Directory.CreateTempSubdirectory("XComUncooker_");

        public static readonly string[] TextureFileCaches = [ "CharTextures", "Lighting", "Textures", "Textures_startup" ];

        static int Main(string[] args)
        {
            Logger.MinLevel = LogLevel.Info;

            Console.WriteLine("Starting up XCOM Uncooker..");
            Console.WriteLine();

            string gameFilesPath, outputPath;

            if (args.Length == 2)
            {
                gameFilesPath = args[0];
                outputPath = args[1];

                if (!Directory.Exists(gameFilesPath))
                {
                    Console.WriteLine($"Error: specified game directory does not exist: {gameFilesPath}");
                    return -1;
                }

                if (!Directory.Exists(outputPath))
                {
                    Console.WriteLine($"Error: specified output directory does not exist: {outputPath}");
                    return -1;
                }
            }
            else
            {
                Console.CursorVisible = true;

                do
                {
                    Console.Write("Enter path to XCOM EW's CookedPCConsole folder: ");
                    gameFilesPath = Console.ReadLine();

                    if (!Directory.Exists(gameFilesPath))
                    {
                        Console.WriteLine("Error: specified directory does not exist.");
                        Console.WriteLine();
                    }
                    else
                    {
                        break;
                    }
                }
                while (true);

                do
                { 
                    Console.Write("Enter path to save output in (this should probably be the UDK's Content folder): ");
                    outputPath = Console.ReadLine();
                    
                    if (!Directory.Exists(outputPath))
                    {
                        Console.WriteLine("Error: specified directory does not exist.");
                        Console.WriteLine();
                    }
                    else
                    {
                        break;
                    }
                }
                while (true);

                Console.WriteLine();
            }

            Console.CursorVisible = false;

            try
            {
                Execute(gameFilesPath, outputPath);
            }
            catch (Exception e)
            {
                Log.Error($"Exception occurred while executing: ${e}");
                Log.EmptyLine();
                Log.EmptyLine();
                Log.Warning("XCOM Uncooker will now close. Attempting to delete the following temp directory, but you should check manually also:");
                Log.Warning($"        {TempDirectory.FullName}");
                return -1;
            }
            finally
            {
                // Always try our best to clean up the temp directory, because it's probably full of decompressed
                // versions of UPK files, meaning it's many gigabytes in size
                Directory.Delete(TempDirectory.FullName, true);
            }

            return 0;
        }

        private static void Execute(string gameFilesPath, string outputPath) 
        {
            var loggerFactory = LoggerFactory.Create(builder =>
            {
                builder.AddConsole();
            });

            using (IUnrealArchiveManager archiveManager = new UnrealArchiveManager(loggerFactory))
            {
                var filePaths = new List<string>();

                if (Directory.Exists(gameFilesPath))
                {
                    var dirFiles = Directory.GetFiles(gameFilesPath, "*.upk", SearchOption.AllDirectories);
                    Log.Verbose($"Treating argument '{gameFilesPath}' as a directory, containing {dirFiles.Count()} UPK files.");

                    filePaths.AddRange(dirFiles);
                }

                filePaths = filePaths.Where(IsSupportedFile).ToList();

                if (filePaths.Count == 0)
                {
                    Log.Error($"Couldn't find any usable UPK files under the path {gameFilesPath}");
                    Log.EmptyLine();
                    Log.Error("Uncooking failed. Aborting..");
                }

#if false
            Log.Info("Attempting to read global shader cache..");
            var globalShaderStream = new UnrealDataReader( File.Open(Path.Combine(folderPath, "GlobalShaderCache-PC-D3D-SM3.bin"), FileMode.Open, FileAccess.Read));
            var globalShaderFile = new GlobalShaderFile(globalShaderStream);
            Log.Info("Done reading global shader cache.");

            // TODO copy data to local shader cache file
            var localShaderStream = new UnrealDataWriter( File.Open(Path.Combine(folderPath, "LocalShaderCache-PC-D3D-SM3.upk"), FileMode.Open, FileAccess.Write));
            var localShaderArchive = new FArchive("LocalShaderCache-PC-D3D-SM3", null);

            // Write the local shader archive to disk
            localShaderArchive.BeginSerialization(localShaderStream);
            localShaderArchive.SerializeHeaderData();
            localShaderArchive.SerializeBodyData(null);
            localShaderStream.Seek(0, SeekOrigin.Begin);
            localShaderArchive.SerializeHeaderData();
            localShaderArchive.EndSerialization();
#endif

                Log.Info("Attempting to load game archives..");
                archiveManager.LoadInputArchives(filePaths);

                Log.Info("Attempting to recreate the uncooked archives..");

                var tfcEntries = new List<TextureFileCacheEntry>();
                foreach (var tfcName in TextureFileCaches)
                {
                    string tfcPath = Path.Combine(gameFilesPath, $"{tfcName}.tfc");
                    tfcEntries.Add(new TextureFileCacheEntry()
                    {
                        FilePath = tfcPath,
                        TextureFileName = tfcName
                    });
                }

                Linker uncookingLinker = archiveManager.UncookArchives(tfcEntries);

                Log.EmptyLine();
                Log.Info("Uncooking process is complete!");
                Log.EmptyLine();
                Log.Info("You're almost done - be sure to continue following the installation instructions.");
            }
        }

        private static bool IsSupportedFile(string path)
        {
            string fileName = Path.GetFileName(path);

            // These are map patches added by Long War, no need to reverse them right now
            if (fileName.StartsWith("patch_"))
            {
                return false;
            }

            if (fileName.Contains("ShaderCache") || fileName.Contains("GlobalPersistentCookerData"))
            {
                return false;
            }

            return true;
        }
    }
}
