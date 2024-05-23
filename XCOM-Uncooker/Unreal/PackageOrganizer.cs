using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XCOM_Uncooker.Unreal.Physical;

namespace XCOM_Uncooker.Unreal
{
    /// <summary>
    /// This class's job is to decide which package assets should end up in once they're uncooked.
    /// </summary>
    public class PackageOrganizer
    {
        private struct PrefixConfig(string prefix, string folder)
        {
            public string Prefix = prefix;
            public string Folder = folder;
        }

        private static readonly Regex VoicePackageRegex = new Regex(@"^(?:Mec)?(Male|Female)Voice\d+_([^._]+)");

        /// <summary>
        /// Almost every uncooked asset will end up inside of this folder, and likely some subfolders.
        /// Maps are in a different top level folder.
        /// </summary>
        public const string RootFolder = "XComGame\\";

        private const string Environments = "Packages\\Environments\\";
        private const string GameData = "Packages\\GameData\\";
        private const string GFx = "Packages\\GFx\\";
        private const string FX = "Packages\\FX\\";
        private const string Maps = "Maps\\";
        private const string Voices = "Packages\\Sound\\Voices\\";
        private const string WaveData = "Sounds\\INT\\";
        private const string WaveDataCue = "Packages\\Sound\\";
        private const string Weapons = "Packages\\Weapons\\";
        private const string UI_3D = "Packages\\UI_3D\\";
        private const string Vehicles = "Packages\\Vehicles\\";

        /// <summary>
        /// Package names which exactly match a key in this map will be assigned to the
        /// folder designated by the corresponding value.
        /// </summary>
        private static readonly IDictionary<string, string> ExactMatches = new Dictionary<string, string>() {
            { "UnitCursor",   UI_3D },
            { "UnitPalettes", GameData }
        };

        private static readonly IList<PrefixConfig> FolderByPrefix =
        [
            new("GameData_",        GameData),
            new("GameUnit_",        GameData + "Units\\"),
            new("Unit_",            GameData + "Units\\"),
            new("Hair_",            GameData + "Hair\\"),
            new("Helmet_",          GameData + "Hats\\"),
            new("Head_",            GameData + "Heads\\"),
            new("Kit_",             GameData + "ArmorKits\\"),
            new("Perk_",            GameData + "Perks\\"),
            new("Weapon_",          GameData + "Weapons\\"),

            new("gfx",              GFx),
            new("UICollection_",    GFx),
            new("UILibrary_",       GFx),

            new("FX_CH",            FX + "Characters\\"),
            new("FX_Cinematic",     FX + "Cinematic\\"),
            new("FX_Destruction",   FX + "Destruction\\"),
            new("FX_Dev",           FX + "Dev\\"),
            new("FX_Environmental", FX + "Environmental\\"),
            new("FX_Psionics",      FX + "Psionics\\"),
            new("FX_MEC_WP",        FX + "Weapons\\"),
            new("FX_WP",            FX + "Weapons\\"),

            new("UI_",              UI_3D),

            new("VEH_",             Vehicles),
            
            new("AnnetteVoice",     Voices),
            new("BSF",              Voices), // Blueshirt female
            new("BSM",              Voices), // Blueshirt male
            new("ZhangVoice",       Voices),
            new("Cryssalid1_Bank",  Voices),
            new("Cyberdisc1_Bank",  Voices),
            new("Drone1_Bank",      Voices),
            new("Elder1_Bank",      Voices),
            new("Exalt1_Bank",      Voices),
            new("Floater1_Bank",    Voices),
            new("Muton1_Bank",      Voices),
            new("Sectoid1_Bank",    Voices),
            new("Sectopod1_Bank",   Voices),
            new("Thinman1_Bank",    Voices),
            new("Zombie1_Bank",     Voices),
            new("Voice_",           Voices),

            // WaveDataCue must come before WaveData to avoid a shorter prefix match
            new("DLC60TacticalWaveDataCue", WaveDataCue),
            new("DLC90TacticalWaveDataCue", WaveDataCue),

            new("MEC",              Weapons),
            new("WP_",              Weapons),
        ];

        public static bool TryMatchPackageToFolders(FArchive archive, out string folderPath)
        {
            if (ExactMatches.TryGetValue(archive.FileName, out string value))
            {
                folderPath = Path.Combine(RootFolder, value);
                return true;
            }

            if (archive.IsMap)
            {
                folderPath = Path.Combine(RootFolder, Maps);
                return true;
            }

            if (TryHandleVoicePackage(archive.FileName, out folderPath))
            {
                folderPath = Path.Combine(RootFolder, folderPath);
                return true;
            }

            for (int i = 0; i < FolderByPrefix.Count; i++)
            {
                if (archive.FileName.StartsWith(FolderByPrefix[i].Prefix))
                {
                    folderPath = Path.Combine(RootFolder, FolderByPrefix[i].Folder);
                    return true;
                }
            }

            folderPath = RootFolder;
            return false;
        }

        private static bool TryHandleVoicePackage(string fileName, out string folderPath)
        {
            var match = VoicePackageRegex.Match(fileName);

            if (match.Success)
            {
                string gender = match.Groups[1].Value;
                string language = match.Groups[2].Value;
                folderPath = $"Packages\\Voices\\{gender}\\{language}\\";

                return true;
            }

            folderPath = "";
            return false;
        }
    }
}
