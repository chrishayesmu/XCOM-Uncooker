using XCOM_Uncooker.Unreal;
using XCOM_Uncooker.Unreal.Physical;

namespace XCOM_Uncooker
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Expected one or more arguments which are file paths to XCOM game packages.");
                return;
            }

            var filePaths = new List<string>();

            foreach (string arg in args)
            {
                if (Directory.Exists(arg))
                {
                    var dirFiles = Directory.GetFiles(arg, "*.upk", SearchOption.AllDirectories);

                    filePaths.AddRange(dirFiles);
                }
                else if (File.Exists(arg))
                {
                    filePaths.Add(arg);
                }
                else
                {
                    throw new ArgumentException($"Argument {arg} does not appear to be a path to a file or directory!");
                }
            }

            // Omit the patch_ files, which are map patches added in LW
            filePaths = filePaths.Where(p => !Path.GetFileName(p).StartsWith("patch_")).ToList();

            Console.WriteLine("Attempting to load game archives..");
            var linker = new Linker(logVerbose: false);
            linker.LoadArchives(filePaths);
            Console.WriteLine("Archive loading is complete.");

            Console.WriteLine("Attempting to recreate the uncooked archives..");
            linker.UncookArchives();
            Console.WriteLine("Uncooking process is complete.");

            /*
            foreach (string arg in args) 
            {
                Console.WriteLine("Opening archive archive file " + arg);
                var stream = File.Open(arg, FileMode.Open);

                var archive = new FArchive(stream);

                Console.WriteLine("Package signature verified..");
                Console.WriteLine("Format version: " + archive.PackageFileSummary.FileVersion);
                Console.WriteLine("Licensee version: " + archive.PackageFileSummary.LicenseeVersion);
                Console.WriteLine("Header size: " + archive.PackageFileSummary.HeaderSize);
                Console.WriteLine("Folder name: " + archive.PackageFileSummary.FolderName);
                Console.WriteLine("PackageFlags: 0x" + archive.PackageFileSummary.PackageFlags.ToString("x"));
                Console.WriteLine("Name count: " + archive.PackageFileSummary.NameCount);
                Console.WriteLine("Name offset: " + archive.PackageFileSummary.NameOffset);
                Console.WriteLine("Export count: " + archive.PackageFileSummary.ExportCount);
                Console.WriteLine("Export offset: " + archive.PackageFileSummary.ExportOffset);
                Console.WriteLine("Import count: " + archive.PackageFileSummary.ImportCount);
                Console.WriteLine("Import offset: " + archive.PackageFileSummary.ImportOffset);
                Console.WriteLine("DependsOffset: " + archive.PackageFileSummary.DependsOffset);
                Console.WriteLine("ThumbnailTableOffset: " + archive.PackageFileSummary.ThumbnailTableOffset);
                Console.WriteLine("PackageGuid: " + archive.PackageFileSummary.PackageGuid);

                Console.WriteLine("GenerationCount: " + archive.PackageFileSummary.Generations.Length);
                for (int i = 0; i < archive.PackageFileSummary.Generations.Length; i++)
                {
                    Console.WriteLine($"    Generations {i}: " + archive.PackageFileSummary.Generations[i]);
                }

                Console.WriteLine("EngineVersion: " + archive.PackageFileSummary.EngineVersion);
                Console.WriteLine("CookerVersion: " + archive.PackageFileSummary.CookerVersion);
                Console.WriteLine("CompressionFlags: 0x" + archive.PackageFileSummary.CompressionFlags.ToString("x"));
                Console.WriteLine("NumCompressedChunks: " + archive.PackageFileSummary.NumCompressedChunks);
                Console.WriteLine("PackageSource: " + archive.PackageFileSummary.PackageSource.ToString("x"));

                Console.WriteLine("AdditionalPackagesToCookCount: " + archive.PackageFileSummary.AdditionalPackagesToCook.Length);
                for (int i = 0; i < archive.PackageFileSummary.AdditionalPackagesToCook.Length; i++)
                {
                    Console.WriteLine($"    AdditionalPackagesToCook {i}: " + archive.PackageFileSummary.AdditionalPackagesToCook[i]);
                }

                Console.WriteLine("NameTable length: " + archive.NameTable.Length);
                for (int i = 0; i < 20 && i < archive.NameTable.Length; i++)
                {
                    Console.WriteLine($"Name {i}: {archive.NameTable[i]}");
                }

                Console.WriteLine("ImportTable length: " + archive.ImportTable.Length);
                for (int i = 0; i < 20 && i < archive.ImportTable.Length; i++)
                {
                    Console.WriteLine($"Import {i}: {archive.ImportTable[i]}");
                }

                Console.WriteLine("ExportTable length: " + archive.ExportTable.Length);
                for (int i = 0; i < 5 && i < archive.ExportTable.Length; i++)
                {
                    Console.WriteLine($"Export {i}: {archive.ExportTable[i]}");
                }

                // Console.WriteLine("ThumbnailMetadataTable length: " + archive.ThumbnailMetadataTable.Length);
                // for (int i = 0; i < 5 && i < archive.ThumbnailMetadataTable.Length; i++)
                // {
                //     Console.WriteLine($"ThumbnailMetadata {i}: {archive.ThumbnailMetadataTable[i].ObjectFullName} @ {archive.ThumbnailMetadataTable[i].FileOffset}");
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
                    // Console.WriteLine($"Referenced class: {className}");
                }
            }
            */
        }
    }
}
