using XCOM_Uncooker.Unreal;
using XCOM_Uncooker.Unreal.Physical;

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
                    var dirFiles = Directory.GetFiles(arg, "*.upk", SearchOption.TopDirectoryOnly);
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

            filePaths = filePaths.Where(IsSupportedFile).ToList();

            Log.Info("Attempting to load game archives..");
            var linker = new Linker();
            linker.LoadArchives(filePaths);
            Log.Info("Archive loading is complete.");

            Log.Info("Attempting to recreate the uncooked archives..");

            // TODO: decide what the arguments to this program are
            string folderPath = args[0];
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

            // Zero interest in reversing this, and no need either
            if (fileName.Contains("RefShaderCache"))
            {
                return false;
            }

            return true;
        }
    }
}
