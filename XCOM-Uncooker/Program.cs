using XCOM_Uncooker.Unreal;
using XCOM_Uncooker.Unreal.Physical;

namespace XCOM_Uncooker
{
    internal class Program
    {
        private static readonly Logger Log = new Logger("XCOM Uncooker");

        static int Main(string[] args)
        {
            Logger.StartBackgroundThread();

            if (args.Length == 0)
            {
                Log.Info("Expected one or more arguments which are file paths to XCOM game packages.");
                return -2;
            }

            Console.CursorVisible = false;

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
                    return -1;
                }
            }

            // Omit the patch_ files, which are map patches added in LW
            filePaths = filePaths.Where(p => !Path.GetFileName(p).StartsWith("patch_")).ToList();

            Log.Info("Attempting to load game archives..");
            var linker = new Linker();
            linker.LoadArchives(filePaths);
            Log.Info("Archive loading is complete.");

            Log.Info("Attempting to recreate the uncooked archives..");
            linker.UncookArchives();
            Log.Info("Uncooking process is complete.");

            return 0;

            /*
            foreach (string arg in args) 
            {
                Log.Info("Opening archive archive file " + arg);
                var stream = File.Open(arg, FileMode.Open);

                var archive = new FArchive(stream);

                Log.Info("Package signature verified..");
                Log.Info("Format version: " + archive.PackageFileSummary.FileVersion);
                Log.Info("Licensee version: " + archive.PackageFileSummary.LicenseeVersion);
                Log.Info("Header size: " + archive.PackageFileSummary.HeaderSize);
                Log.Info("Folder name: " + archive.PackageFileSummary.FolderName);
                Log.Info("PackageFlags: 0x" + archive.PackageFileSummary.PackageFlags.ToString("x"));
                Log.Info("Name count: " + archive.PackageFileSummary.NameCount);
                Log.Info("Name offset: " + archive.PackageFileSummary.NameOffset);
                Log.Info("Export count: " + archive.PackageFileSummary.ExportCount);
                Log.Info("Export offset: " + archive.PackageFileSummary.ExportOffset);
                Log.Info("Import count: " + archive.PackageFileSummary.ImportCount);
                Log.Info("Import offset: " + archive.PackageFileSummary.ImportOffset);
                Log.Info("DependsOffset: " + archive.PackageFileSummary.DependsOffset);
                Log.Info("ThumbnailTableOffset: " + archive.PackageFileSummary.ThumbnailTableOffset);
                Log.Info("PackageGuid: " + archive.PackageFileSummary.PackageGuid);

                Log.Info("GenerationCount: " + archive.PackageFileSummary.Generations.Length);
                for (int i = 0; i < archive.PackageFileSummary.Generations.Length; i++)
                {
                    Log.Info($"    Generations {i}: " + archive.PackageFileSummary.Generations[i]);
                }

                Log.Info("EngineVersion: " + archive.PackageFileSummary.EngineVersion);
                Log.Info("CookerVersion: " + archive.PackageFileSummary.CookerVersion);
                Log.Info("CompressionFlags: 0x" + archive.PackageFileSummary.CompressionFlags.ToString("x"));
                Log.Info("NumCompressedChunks: " + archive.PackageFileSummary.NumCompressedChunks);
                Log.Info("PackageSource: " + archive.PackageFileSummary.PackageSource.ToString("x"));

                Log.Info("AdditionalPackagesToCookCount: " + archive.PackageFileSummary.AdditionalPackagesToCook.Length);
                for (int i = 0; i < archive.PackageFileSummary.AdditionalPackagesToCook.Length; i++)
                {
                    Log.Info($"    AdditionalPackagesToCook {i}: " + archive.PackageFileSummary.AdditionalPackagesToCook[i]);
                }

                Log.Info("NameTable length: " + archive.NameTable.Length);
                for (int i = 0; i < 20 && i < archive.NameTable.Length; i++)
                {
                    Log.Info($"Name {i}: {archive.NameTable[i]}");
                }

                Log.Info("ImportTable length: " + archive.ImportTable.Length);
                for (int i = 0; i < 20 && i < archive.ImportTable.Length; i++)
                {
                    Log.Info($"Import {i}: {archive.ImportTable[i]}");
                }

                Log.Info("ExportTable length: " + archive.ExportTable.Length);
                for (int i = 0; i < 5 && i < archive.ExportTable.Length; i++)
                {
                    Log.Info($"Export {i}: {archive.ExportTable[i]}");
                }

                // Log.Info("ThumbnailMetadataTable length: " + archive.ThumbnailMetadataTable.Length);
                // for (int i = 0; i < 5 && i < archive.ThumbnailMetadataTable.Length; i++)
                // {
                //     Log.Info($"ThumbnailMetadata {i}: {archive.ThumbnailMetadataTable[i].ObjectFullName} @ {archive.ThumbnailMetadataTable[i].FileOffset}");
                // }

                var classNames = new HashSet<string>();
                for (int i = 0; i < archive.ImportTable.Length; i++)
                {
                    classNames.Add(archive.NameToString(archive.ImportTable[i].ClassName));
                }

                for (int i = 0; i < archive.ExportTable.Length; i++)
                {
                    if (archive.ExportTable[i].ClassIndex == 0)
                    {
                        classNames.Add(archive.NameToString(archive.ExportTable[i].ObjectName));
                    }
                }

                foreach (var className in classNames) 
                {
                    // Log.Info($"Referenced class: {className}");
                }
            }
            */
        }
    }
}
