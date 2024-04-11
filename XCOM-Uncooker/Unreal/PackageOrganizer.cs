using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        /// <summary>
        /// Every uncooked asset will end up inside of this folder, and likely some subfolders.
        /// </summary>
        public const string RootFolder = "XComGame/Content/";

        private const string GameDataFolder = "Packages/GameData/";
        private const string GFxFolder = "Packages/GFx/";
        private const string FXFolder = "Packages/FX/";
        private const string VoicesFolder = "Packages/Sound/Voices/";
        private const string WaveDataFolder = "Sounds/INT/";
        private const string WaveDataCueFolder = "Packages/Sound/";
        private const string WeaponsFolder = "Packages/Weapons/";
        private const string UIFolder = "Packages/UI/";
        private const string VehiclesFolder = "Packages/Vehicles/";

        private static readonly IList<PrefixConfig> FolderByPrefix =
        [
            new("GameData_",        GameDataFolder),
            new("GameUnit_",        GameDataFolder + "Units/"),
            new("WP_",              GameDataFolder + "Weapons/"),
            new("gfx",              GFxFolder),
            new("FX_CH",            FXFolder + "Characters/"),
            new("FX_Cinematic",     FXFolder + "Cinematic/"),
            new("FX_Destruction",   FXFolder + "Destruction/"),
            new("FX_Dev",           FXFolder + "Dev/"),
            new("FX_Environmental", FXFolder + "Environmental/"),
            new("FX_Psionics",      FXFolder + "Psionics/"),
            new("FX_MEC_WP",        FXFolder + "Weapons/"),
            new("FX_WP",            FXFolder + "Weapons/"),
            new("VEH_",             VehiclesFolder),
            new("AnnetteVoice",     VoicesFolder),
            new("BSF",              VoicesFolder), // Blueshirt female
            new("BSM",              VoicesFolder), // Blueshirt male
            new("ZhangVoice",       VoicesFolder),
            new("Cryssalid1_Bank",  VoicesFolder),
            new("Cyberdisc1_Bank",  VoicesFolder),
            new("Drone1_Bank",      VoicesFolder),
            new("Elder1_Bank",      VoicesFolder),
            new("Exalt1_Bank",      VoicesFolder),
            new("Floater1_Bank",    VoicesFolder),
            new("Muton1_Bank",      VoicesFolder),
            new("Sectoid1_Bank",    VoicesFolder),
            new("Sectopod1_Bank",   VoicesFolder),
            new("Thinman1_Bank",    VoicesFolder),
            new("Zombie1_Bank",     VoicesFolder),
            new("Voice_",           VoicesFolder),

            // WaveDataCue must come before WaveData to avoid a shorter prefix match
            new("DLC60TacticalWaveDataCue", WaveDataCueFolder),
            new("DLC90TacticalWaveDataCue", WaveDataCueFolder),

            new( "Weapon_",         WeaponsFolder),
        ];

        public static bool TryMatchPackageToFolders(string packageName, ref string folderPath)
        {
            folderPath = "";

            for (int i = 0; i < FolderByPrefix.Count; i++)
            {
                if (packageName.StartsWith(FolderByPrefix[i].Prefix))
                {
                    folderPath = FolderByPrefix[i].Folder;
                    return true;
                }
            }

            return false;
        }
    }
}
