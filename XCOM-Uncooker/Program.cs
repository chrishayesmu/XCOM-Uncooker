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

            string folderPath = args[0];
            filePaths = filePaths.Where(IsSupportedFile).ToList();

#if false
            Log.Info("Attempting to read global shader cache..");
            var globalShaderStream = new UnrealDataReader(File.Open(Path.Combine(folderPath, "GlobalShaderCache-PC-D3D-SM3.bin"), FileMode.Open, FileAccess.Read));
            var globalShaderFile = new GlobalShaderFile(globalShaderStream);
            Log.Info("Done reading global shader cache.");

            // TODO copy data to local shader cache file
            var localShaderStream = new UnrealDataWriter(File.Open(Path.Combine(folderPath, "LocalShaderCache-PC-D3D-SM3.upk"), FileMode.Open, FileAccess.Write));
            var localShaderArchive = new FArchive("LocalShaderCache-PC-D3D-SM3", null);

            // Write the local shader archive to disk
            localShaderArchive.BeginSerialization(localShaderStream);
            localShaderArchive.SerializeHeaderData();
            localShaderArchive.SerializeBodyData(null);
            localShaderStream.Seek(0, SeekOrigin.Begin);
            localShaderArchive.SerializeHeaderData();
            localShaderArchive.EndSerialization();
#endif

#if true
            Log.Info("Attempting to read ref shader cache..");
            var refShaderStream = new UnrealDataReader(File.Open(Path.Combine(folderPath, "RefShaderCache-PC-D3D-SM3.upk"), FileMode.Open, FileAccess.Read));
            var refShaderArchive = new FArchive("RefShaderCache-PC-D3D-SM3", null);

            refShaderArchive.BeginSerialization(refShaderStream);
            refShaderArchive.SerializeHeaderData();
            refShaderArchive.SerializeBodyData(null);
            refShaderArchive.EndSerialization();

            var localShaderStream = new UnrealDataReader(File.Open(Path.Combine(folderPath, "LocalShaderCache-PC-D3D-SM3.upk"), FileMode.Open, FileAccess.Read));
            var localShaderArchive = new FArchive("LocalShaderCache-PC-D3D-SM3", null);

            localShaderArchive.BeginSerialization(localShaderStream);
            localShaderArchive.SerializeHeaderData();
            localShaderArchive.SerializeBodyData(null);
            localShaderArchive.EndSerialization();

            var refShaderCache = refShaderArchive.ExportedObjects[0] as UShaderCache;
            var localShaderCache = localShaderArchive.ExportedObjects[0] as UShaderCache;

            /*
            FShaderCacheEntry defaultValue = default;
            var unmatchedEntries = new List<FShaderCacheEntry>();
            var matchedEntries = new List<FShaderCacheEntry>();
            var sameHashEntries = new List<FShaderCacheEntry>();
            var differentHashEntries = new List<FShaderCacheEntry>();

            foreach (var entry in refShaderCache.ShaderCache.CacheEntries)
            {
                var matchingEntry = localShaderCache.ShaderCache.CacheEntries.FirstOrDefault(ent => ent.ShaderId == entry.ShaderId, defaultValue);

                if (matchingEntry.ShaderId == defaultValue.ShaderId)
                {
                    unmatchedEntries.Add(entry);
                    continue;
                }

                matchedEntries.Add(entry);
            } */

            Log.Info("Done reading ref shader cache.");

            Log.Info("Attempting to write modified shader cache..");

            // Normally you can't just turn an archive around and serialize it back out,
            // but the RefShaderCache is a single export object that we're only changing
            // native data in, so we can get away with this hack.
            var refShaderStreamWrite = new UnrealDataWriter(File.Open(Path.Combine("archives", "RefShaderCache-PC-D3D-SM3.upk"), FileMode.Create));

            refShaderArchive.BeginSerialization(refShaderStreamWrite);
            refShaderArchive.SerializeHeaderData();
            refShaderArchive.SerializeBodyData(null);
            refShaderStreamWrite.Seek(0, SeekOrigin.Begin);
            refShaderArchive.SerializeHeaderData();
            refShaderArchive.EndSerialization();

            Log.Info("Done writing modified ref shader cache.");
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

            return 0;
        }

        private static bool IsSupportedFile(string path)
        {
            string fileName = Path.GetFileName(path);

            // These are map patches added by Long War, no need to reverse them right now
            if (fileName.StartsWith("patch_"))
            {
                return false;
            }

            // We handle the ref shader cache in a separate way from other files
            if (fileName.Contains("RefShaderCache"))
            {
                return false;
            }

            return true;
        }
    }
}
