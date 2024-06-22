using System.IO;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal;
using XCOM_Uncooker.Unreal.Physical;
using XCOM_Uncooker.Unreal.Shaders;

namespace XCOM_Uncooker
{
    internal class Program
    {
        private static readonly Logger Log = new Logger("XCOM Uncooker");

        // Temp directory which will contain uncompressed versions of compressed archives, if needed.
        // The main Program.cs is responsible for deleting this when the process exits.
        public static readonly DirectoryInfo TempDirectory = Directory.CreateTempSubdirectory("XComUncooker_");

        static int Main(string[] args)
        {
            Logger.MinLevel = LogLevel.Info;
            Logger.StartBackgroundThread();

            if (args.Length == 0)
            {
                Log.Info("Expected one or more arguments which are file paths to XCOM game packages.");
                return -2;
            }

            Console.CursorVisible = false;

            try
            {
                Execute(args);
            }
            catch (Exception e)
            {
                Log.Error($"Exception occurred while executing: ${e}");
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

        private static void Execute(string[] args) 
        {
            var filePaths = new List<string>();

            foreach (string arg in args)
            {
                if (Directory.Exists(arg))
                {
                    var dirFiles = Directory.GetFiles(arg, "*.upk", SearchOption.AllDirectories);
                    Log.Verbose($"Treating argument '{arg}' as a directory, containing {dirFiles.Count()} UPK files.");

                    filePaths.AddRange(dirFiles);
                }
                else if (File.Exists(arg))
                {
                    filePaths.Add(arg);
                }
                else
                {
                    Log.Error($"Argument '{arg}' does not appear to be a path to a file or directory!");
                    return;
                }
            }

            string folderPath = args[0];
            filePaths = filePaths.Where(IsSupportedFile).ToList();

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
            var linker = new Linker();
            linker.LoadArchives(filePaths);
            Log.Info("Archive loading is complete.");

            Log.Info("Attempting to recreate the uncooked archives..");

            // TODO: decide what the arguments to this program are
            linker.RegisterTextureFileCache("CharTextures", Path.Combine(folderPath, "CharTextures.tfc"));
            linker.RegisterTextureFileCache("Lighting", Path.Combine(folderPath, "Lighting.tfc"));
            linker.RegisterTextureFileCache("Textures", Path.Combine(folderPath, "Textures.tfc"));
            linker.RegisterTextureFileCache("Textures_startup", Path.Combine(folderPath, "Textures_startup.tfc"));
            linker.UncookArchives();
            Log.Info("Uncooking process is complete.");
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
