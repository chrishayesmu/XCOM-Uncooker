using Konsole;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core;
using XCOM_Uncooker.Utils;

namespace XCOM_Uncooker.Unreal
{
    public class Linker
    {
        private static readonly Logger Log = new Logger(nameof(Linker));

        // List of all maps files in the game; just used for easy hard-coding with UncookOnly
        private static readonly List<string> AllMapFiles = ["Act1_IntroLevel", "Act1_IntroLevel_Anim", "Act1_IntroLevel_Script", "Act2_Bar_script", "Act2_Pier", "Act2_Pier_Asian",  "Act2_Pier_script", "AddonDebug_AccessLift", "Addon_AccessLift_Excavated", "Addon_AlienContainment", "Addon_Cave", "Addon_CyberneticsLab", "Addon_CyberneticsLab_CAP", "Addon_EleriumPowerGenerator", "Addon_Excavated", "Addon_Excavated_Thermal", "Addon_Foundry", "Addon_Genelab", "Addon_GollopChamber", "Addon_HyperwaveRadar", "Addon_Laboratory", "Addon_LargeRadar", "Addon_OfficerTrainingSchool", "Addon_PowerGenerator", "Addon_PsionicLab", "Addon_SmallRadar", "Addon_SolidRock", "Addon_ThermalCave", "Addon_ThermalPower", "Addon_Workshop", "AlienBase01", "AlienBase02", "Anim_AlienContainment", "Anim_Armory", "Anim_Barracks", "Anim_EleriumGenerator", "Anim_Engineering", "Anim_Foundry", "Anim_GeneLab", "Anim_GollopChamber", "Anim_Hangar", "Anim_Hangar_LaunchBays", "Anim_HyperwaveRadar", "Anim_Laboratory", "Anim_LargeRadar", "Anim_MissionControl", "Anim_OfficerTrainingSchool", "Anim_PowerGenerator", "Anim_PsionicLab", "Anim_ScienceLabs", "Anim_ScienceLabs_Hyperwave", "Anim_SmallRadar", "Anim_ThermalPower", "Anim_Workshop", "BattleOverMissionControl", "CAbductor_Cliffside", "CAbductor_ScorchedEarth", "CB2_MP_Blank", "CBattleship_01", "CBattleship_02", "CIN_Containment_ELD", "CIN_Containment_FLO", "CIN_Containment_HeavyFLO", "CIN_Containment_MUT", "CIN_Containment_MUTBerserk", "CIN_Containment_MUTElite", "CIN_Containment_SEC", "CIN_Containment_SECCom", "CIN_Containment_THN", "CIN_Cryssalid", "CIN_Cryssalid_GC", "CIN_Cyberdisc", "CIN_Cyberdisc_GC", "CIN_DLC1_ZhangIntro", "CIN_DLC1_ZhangOutro", "CIN_DLC2_Annette", "CIN_DLC2_Furies", "CIN_Drone", "CIN_Drone_GC", "CIN_DropshipIntros", "CIN_DropshipIntros_Intro", "CIN_Ethereal", "CIN_Ethereal_GC", "CIN_Exalt", "CIN_FastropeIntros", "CIN_Floater", "CIN_Floater_GC", "CIN_FundingCouncil", "CIN_HQLoadScreen", "CIN_LoadScreen", "CIN_MEC", "CIN_Mechtoid", "CIN_Muton", "CIN_Muton_GC", "CIN_Outsider", "CIN_Sectoid", "CIN_Sectoid_GC", "CIN_Sectopod", "CIN_Sectopod_GC", "CIN_Seeker", "CIN_Shiv", "CIN_Soldier", "CIN_ThinMan", "CIN_ThinMan_GC", "CIN_TP05A_EnteringAlienBase", "CIN_TP09_Psionics", "CIN_TP09_Psionics_Fixed", "CIN_TP11_PreOutro", "CIN_TP12A_ArriveTempleShip", "CIN_TP12_EtherealReveal", "CIN_TP13_Outro", "CIN_XEW_1stGeneMod", "CIN_XEW_1stMEC", "CIN_XEW_Facemelt", "CIN_Zombie", "CIN_Zombie_GC", "CLrgScout_City", "CLrgScout_DeepWoods", "CLrgScout_ForestTrench", "CLrgScout_Stonewall", "CLrgScout_TheBarrens", "Command1", "COverseer_DeepWoods", "COverseer_ForestTrench", "COverseer_Stonewall", "COverseer_TheBarrens", "CSmallScout_Badlands", "CSmallScout_DirtRoad", "CSmallScout_DirtRoad_Tutorial", "CsmallScout_Farm", "CSmallScout_Marshlands", "CSmallScout_NukedCity", "CSmallScout_Quagmire", "CSmallScout_Roadhouse", "CSupplyShip_OverlookA", "CSupplyShip_Wildfire", "DEMO_SonOfFacemelt", "DEMO_SonOfFacemelt_Stream", "DLC1_1_LowFriends", "DLC1_1_LowFriends_Stream", "DLC1_2_CnfndLight", "DLC1_2_CnfndLight_Stream", "DLC1_3_Gangplank", "DLC1_3_Gangplank_Stream", "DLC2_1_Portent", "DLC2_1_Portent_CovExt", "DLC2_1_Portent_Stream", "DLC2_2_Deluge", "DLC2_2_Deluge_Stream", "DLC2_3_Furies", "DLC2_3_Furies_Stream", "EWI_ChryssalidHive", "EWI_ChryssalidHive_Stream", "EWI_ExaltHQAssault", "EWI_ExaltHQAssault_Stream", "EWI_HQAssault", "EWI_HQAssault_MP", "EWI_HQAssault_Stream", "EWI_MeldTutorial", "EWI_MeldTutorial_Stream", "LAbductor_FarmOutskirts", "LAbductor_WindingStream", "LLrgScout_CreepyForest", "LLrgScout_Hillside", "LSmallScout_River", "LSmallScout_Rivervalley", "LSupplyShip_ForestGrove", "LSupplyShip_RockyGorge", "Soldier2Spotted", "Static_Armory", "Static_Barracks", "Static_Engineering", "Static_Hangar", "Static_Hangar_CAP", "Static_Infirmary", "Static_MissionControl", "Static_ScienceLabs", "Static_SituationRoom", "SurvivorReturnsMC", "TempleShip", "TempleShip_Stream", "URB_Bar", "URB_Bar_Terror", "URB_Boulevard", "URB_Boulevard_CNH", "URB_Boulevard_Euro", "URB_CemeteryGrand", "URB_CemeteryGrand_Bomb", "URB_CemeteryGrand_Stream", "URB_CommercialAlley", "URB_CommercialAlley_CovExt", "URB_CommercialAlley_EWI", "URB_CommercialAlley_Stream", "URB_CommercialAlley_Terror", "URB_CommercialRestaurant", "URB_CommercialRestaurant_CNH", "URB_CommercialStreet", "URB_CommercialStreet_Terror", "URB_ConvienienceStore", "URB_ConvienienceStore_CovExt", "URB_ConvienienceStore_EWI", "URB_ConvienienceStore_Terror", "URB_Demolition_CNH", "URB_Demolition_EWI", "URB_FastFood", "URB_FastFood_EWI", "URB_GasStation_CNH", "URB_GasStation_EWI", "URB_Highway1", "URB_HIghwayBomb_Stream", "URB_HighwayBridge", "URB_HighwayConstruction", "URB_HighwayConstruction_CNH", "URB_HighwayConstruction_CovExt", "URB_HighwayConstruction_EWI", "URB_HighwayFallen", "URB_HighwayFallen_Stream", "URB_IndustrialOffice", "URB_IndustrialOffice_CNH", "URB_IndustrialOffice_Stream", "URB_LiquorStore", "URB_LiquorStore_CovExt", "URB_MilitaryAmmo", "URB_MilitaryAmmo_CovExt", "URB_OfficePaper", "URB_OfficePaper_CNH", "URB_OfficePaper_EWI", "URB_OfficePaper_Terror", "URB_PierA", "URB_PierA_Asian", "URB_PierA_Stream", "URB_PierA_StreamB", "URB_PierA_Terror", "URB_PierA_Terror_CovExt", "URB_PoliceStation", "URB_PoliceStation_CNH", "URB_ResearchOutpost", "URB_ResearchOutpost_CovExt", "URB_ResearchOutpost_EWI", "URB_ResearchOutpost_Rescue", "URB_ResearchOutpost_Stream", "URB_RooftopsConst", "URB_RooftopsConst_Asian", "URB_RooftopsConst_CNH", "URB_SlaughterhouseA", "URB_Slaughterhouse_Stream", "URB_SmallCemetery", "URB_StreetHurricane", "URB_StreetHurricane_Stream", "URB_StreetHurricane_Terror", "URB_StreetOverpass", "URB_StreetOverpass_CovExt", "URB_StreetOverpass_EWI", "URB_StreetOverpass_Stream", "URB_TrainStation", "URB_TrainStation_Stream", "URB_Trainyard", "URB_Trainyard_CovExt", "URB_Trainyard_Stream", "URB_TruckStop", "URB_TruckStop_CNH", "URB_TruckStop_EWI", "URB_TruckStop_Stream", "XComShell" ];

        public static readonly List<string> FilesToSkip = [];

        public static readonly List<string> UncookOnly = [];

        public static readonly List<string> NeverUncook = [ "Core", "Engine", "GameFramework", "GFxUI", "GFxUIEditor", "IpDrv", "OnlineSubsystemSteamworks", "XComGame", "XComStrategyGame", "XComUIShell" ];

        public List<FArchive> InputArchives;
        public FArchive[] OutputArchives;

        public IDictionary<string, MultiValueDictionary<string, UObject>> ObjectsByUncookedArchiveName;
        public IDictionary<string, string> UncookedArchiveNameByObjectPath;
        public IDictionary<Guid, UPackage> InputPackagesByGuid;

        private IDictionary<string, FileStream> TextureFileCaches = new Dictionary<string, FileStream>();
        private ConcurrentQueue<ProgressBar> ParallelProgressBars = [];

        public void LoadArchives(List<string> filePaths)
        {
            var validPaths = filePaths.Where(path => Path.Exists(path) && !FilesToSkip.Contains(Path.GetFileName(path))).ToList();

            InputArchives = new List<FArchive>(validPaths.Count);

            int numArchivesCompleted = 0, numSucceeded = 0, numFailed = 0;

            #region Deserialize archive headers

            Log.Info($"Attempting to read archive headers for {validPaths.Count} archives..");

            // Create the archives before the parallelization, makes the parallel bit simpler
            for (int i = 0; i < validPaths.Count; i++) 
            {
                var stream = new UnrealDataReader( File.Open(validPaths[i], FileMode.Open, FileAccess.Read));
                InputArchives.Add(new FArchive(Path.GetFileNameWithoutExtension(validPaths[i]), this));
                InputArchives[i].BeginSerialization(stream);
            }

            numArchivesCompleted = 0;
            var compressedArchives = new ConcurrentQueue<FArchive>();
            var progressBar = new ProgressBar(PbStyle.SingleLine, validPaths.Count);

            Parallel.ForEach(InputArchives, (archive) =>
            {
                try
                {
                    archive.SerializeHeaderData();
                    Interlocked.Increment(ref numSucceeded);

                    if (archive.IsBodyCompressed || archive.IsFullyCompressed)
                    {
                        compressedArchives.Enqueue(archive);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error($"Could not initialize archive \"{archive.FileName}\" - it will be skipped.");
                    Log.Info(ex.ToString());
                    Interlocked.Increment(ref numFailed);
                }
                finally
                {
                    progressBar.Refresh(Interlocked.Increment(ref numArchivesCompleted), archive.FileName);
                }
            });

            Log.EmptyLine();
            Log.Info($"Done parsing archive headers. {numSucceeded} succeeded and {numFailed} failed.");

            if (!compressedArchives.IsEmpty)
            {
                Log.Info($"{compressedArchives.Count} archives are compressed. Decompressed versions will be copied to a temporary directory:");
                Log.Info($"        {Program.TempDirectory.FullName}");

                progressBar = new ProgressBar(PbStyle.SingleLine, compressedArchives.Count);

                numArchivesCompleted = 0;
                var decompressedArchives = new ConcurrentQueue<FArchive>();

                Parallel.ForEach(compressedArchives, new ParallelOptions() { MaxDegreeOfParallelism = 4 }, (archive) =>
                {
                    string archivePath = Path.Combine(Program.TempDirectory.FullName, archive.FileName + ".upk");
                    
                    using (UnrealDataWriter tempFileStream = new UnrealDataWriter(File.Open(archivePath, FileMode.Create)))
                    {
                        archive.DecompressToStream(tempFileStream);
                        archive.EndSerialization();
                    }

                    // Create a new archive and catch it up to where any previous archives are. Replace the previous stream
                    // with a read-only one, because having a write stream open will interfere with cleaning up the temp directory.
                    var fileStream = File.Open(archivePath, FileMode.Open, FileAccess.Read);
                    var decompressedArchive = new FArchive(archive.FileName, this);
                    decompressedArchive.BeginSerialization(new UnrealDataReader(fileStream));
                    decompressedArchive.SerializeHeaderData();

                    decompressedArchives.Enqueue(decompressedArchive);

                    progressBar.Refresh(Interlocked.Increment(ref numArchivesCompleted), archive.FileName);
                });

                InputArchives.RemoveAll(archive => compressedArchives.Contains(archive));
                InputArchives.AddRange(decompressedArchives);

                // We don't need the compressed archives anymore, make them eligible for GC
                compressedArchives = null;

            }

            #endregion

            #region Deserialize exported classes

            Log.Info("Beginning deserialization of exported classes for archives..");

            progressBar = new ProgressBar(PbStyle.SingleLine, InputArchives.Count);

            numArchivesCompleted = 0;

            Parallel.ForEach(InputArchives, (archive) =>
            {
                if (archive == null)
                {
                    return;
                }

                archive.SerializeClassExports();

                progressBar.Refresh(Interlocked.Increment(ref numArchivesCompleted), archive.FileName);
            });

            Log.EmptyLine();
            Log.Info("Deserialization of exported classes is complete.");

            #endregion

            #region Deserialize export objects

            Log.Info("Beginning deserialization of export objects for archives..");

            var progressBarWindow = new Window(Console.WindowWidth, 1);
            progressBar = new ProgressBar(progressBarWindow, PbStyle.SingleLine, InputArchives.Count);
            
            numArchivesCompleted = 0;

            Parallel.ForEach(InputArchives, (archive) =>
            {
                if (archive == null) 
                { 
                    return; 
                }

                var localProgressBar = GetOrCreateProgressBar(archive.PackageFileSummary.ExportCount);
                archive.SerializeBodyData(localProgressBar);
                ParallelProgressBars.Enqueue(localProgressBar); // return to the pool

                progressBar.Refresh(Interlocked.Increment(ref numArchivesCompleted), "Archives completed");
            });

            // End serialization after all archives are done, due to some threading issues I'm too lazy to fix
            foreach (var archive in InputArchives)
            {
                archive.EndSerialization();
            }

            #endregion

            Log.EmptyLine();
            Log.Info("Deserialization of export objects is complete.");
        }

        /// <summary>
        /// Finds the object specified by the given path, as long as it exists in at least one of the currently-loaded archives.
        /// </summary>
        /// <param name="fullObjectPath">The full object path, as from <see cref="FObjectTableEntry.FullObjectPath"/>.</param>
        /// <param name="tableEntry">Optionally, a table entry for an existing copy of the object. If provided, it will be used
        /// to help disambiguate scenarios where the same object path is used for multiple objects.</param>
        /// <returns>The corresponding <see cref="UObject"/> if found, else null.</returns>
        /// <remarks>
        /// The return value of this is ambiguous if there are multiple objects with the same path. UE3 itself seems to
        /// have the same problem, and explicitly recommends against naming two objects with the same path. For uncooking, this
        /// will occur naturally because the same logical object has been pulled into multiple archives; we simply assume that
        /// all copies of the object are the same and use the first match we find. The exception is if the tableEntry param is set,
        /// in which case we only match objects if they have the same class as the table entry.
        /// </remarks>
        public UObject GetCookedObjectByPath(string fullObjectPath, FObjectTableEntry tableEntry = null)
        {
            foreach (var archive in InputArchives)
            {
                var obj = archive.GetExportedObjectByPath(fullObjectPath, tableEntry);

                if (obj != null)
                {
                    if (tableEntry != null)
                    {
                        if (obj.TableEntry.ClassName != tableEntry.ClassName)
                        {
                            continue;
                        }
                    }

                    return obj;
                }
            }

            return null;
        }

        public string GetTopLevelPackageName(FObjectTableEntry tableEntry)
        {
            var outermost = tableEntry.Outermost;

            if (outermost is FImportTableEntry importEntry)
            {
                return importEntry.ObjectName;
            }
            else
            {
                FExportTableEntry exportEntry = outermost as FExportTableEntry;

                // PackageGuid should only be set for top-level package objects
                if (InputPackagesByGuid != null && exportEntry.PackageGuid != Guid.Empty)
                {
                    if (InputPackagesByGuid.TryGetValue(exportEntry.PackageGuid, out UPackage pkg))
                    {
                        return pkg.NormalizedName;
                    }
                }

                // If we got here, either there's no GUID or it's unrecognized; we hope it's the former
                if (exportEntry.IsPackage)
                {
                    return exportEntry.ObjectName;
                }

                // A top-level non-package object belongs to the implied package of its archive name; normally this
                // should only happen for maps, classes, and maybe weird situations in Core/Engine (I think?)
                return exportEntry.Archive.NormalizedName;
            }
        }

        /// <summary>
        /// Finds the name of the archive which will ultimately contain the given object once it is uncooked.
        /// </summary>
        /// <param name="fullObjectPath"></param>
        /// <returns>The archive's name, or an empty string if no match is found.</returns>
        public string GetUncookedArchiveNameForObject(string fullObjectPath)
        {
            if (UncookedArchiveNameByObjectPath.TryGetValue(fullObjectPath, out string archiveName))
            {
                return archiveName;
            }

            foreach (var entry in ObjectsByUncookedArchiveName)
            {
                if (entry.Value.ContainsKey(fullObjectPath))
                {
                    return entry.Key;
                }
            }

            return "";
        }

        public byte[] ReadTextureData(string textureFileName, int startPosition, int numBytes)
        {
            if (TextureFileCaches.TryGetValue(textureFileName, out FileStream stream))
            {
                byte[] buffer = new byte[numBytes];

                lock (stream)
                {
                    stream.Seek(startPosition, SeekOrigin.Begin);
                    stream.Read(buffer, 0, numBytes);

                    return buffer;
                }
            }
            
            throw new ArgumentException($"Unrecognized TFC file requested: {textureFileName}");
        }

        public void RegisterTextureFileCache(string textureFileName, string filePath)
        {
            TextureFileCaches[textureFileName] = File.OpenRead(filePath);
        }

        public void UncookArchives(string outputPath)
        {
            InputPackagesByGuid = new Dictionary<Guid, UPackage>();

            // Start by finding all of the top-level packages that will exist in the uncooked data set
            var allPackages = new HashSet<string>() { "Core", "Engine" };
            var packageGuids = new Dictionary<Guid, Dictionary<string, string>>();
            foreach (var archive in InputArchives)
            {
                if (archive == null)
                {
                    continue;
                }

                // TODO: this is probably not needed, leaving it in case it is and I forget about it
                allPackages.Add(archive.NormalizedName);

                if (archive.IsMap)
                {
                    allPackages.Add(archive.NormalizedName);
                }

                archive.TopLevelPackages.ForEach(p => { 
                    allPackages.Add(p.NormalizedName);

                    if (p.ExportTableEntry.PackageGuid != Guid.Empty)
                    {
                        InputPackagesByGuid[p.ExportTableEntry.PackageGuid] = p;
                    }

                    if (!packageGuids.ContainsKey(p.ExportTableEntry.PackageGuid))
                    {
                        packageGuids[p.ExportTableEntry.PackageGuid] = new Dictionary<string, string>();
                    }

                    packageGuids[p.ExportTableEntry.PackageGuid][p.NormalizedName] = p.ObjectName;
                });
            }

#if false
            using (var writer = new StreamWriter("packages-and-guids.csv"))
            {
                writer.WriteLine("Package Name,Normalized Name,GUID");

                foreach (var outerEntry in packageGuids)
                {
                    foreach (var innerEntry in outerEntry.Value)
                    {
                        writer.WriteLine($"{innerEntry.Value},{innerEntry.Key},{outerEntry.Key}");
                    }
                }
            }
#endif

            Log.Info($"Found {allPackages.Count} distinct top level packages.");

            // Initialize storage for each of these, arranged by package
            ObjectsByUncookedArchiveName = new Dictionary<string, MultiValueDictionary<string, UObject>>();
            UncookedArchiveNameByObjectPath = new Dictionary<string, string>();
            
            foreach (var package in allPackages)
            {
                ObjectsByUncookedArchiveName.Add(package, new MultiValueDictionary<string, UObject>());
            }

            // Now iterate every package's exports, assigning them to their original source archive
            int skippedArchives = 0, numObjects = 0, skippedObjects = 0, repeatObjects = 0;
            foreach (var archive in InputArchives)
            {
                if (archive == null)
                {
                    continue;
                }

                foreach (var exportObj in archive.ExportedObjects)
                {
                    string fullObjectPath = exportObj.FullObjectPath;
                    string topPackage = fullObjectPath.Split(".")[0];

                    if (exportObj is UPackage && exportObj.ObjectName == archive.FileName)
                    {
                        continue;
                    }

                    // VisGroupActor and XComWorldDataContainer are XCOM-only intrinsic classes which we can't replicate in the UDK.
                    // Having them present in an archive will result in a crash. Here, we check IsMap first because these objects can
                    // only exist in maps, and that check is much cheaper than the class name check.
                    if (archive.IsMap && (exportObj.TableEntry.ClassName == "VisGroupActor" || exportObj.TableEntry.ClassName == "XComWorldDataContainer"))
                    {
                        continue;
                    }

                    // Any object where the outermost object is TheWorld is an object within a map, so direct it to the map's archive
                    if (topPackage == "TheWorld")
                    {
                        topPackage = archive.NormalizedName;
                    }

                    // If an object's top level package doesn't exist in the map, and we add it here instead of skipping the object,
                    // things start crashing in the UDK. A lot. Unfortunately I kinda forgot why so we're just skipping those objects
                    if (!ObjectsByUncookedArchiveName.ContainsKey(topPackage))
                    {
                        skippedObjects++;
                        continue;
                    }

                    // If this object already exists, it must've been exported by another archive also, in which
                    // case we assume the objects are identical and just use whichever one got there first
                    if (ObjectsByUncookedArchiveName[topPackage].TryGetValue(fullObjectPath, out var objectsWithSamePath))
                    {
                        bool isRepeat = false;

                        for (int i = 0; i < objectsWithSamePath.Count; i++)
                        {
                            if (objectsWithSamePath[i].TableEntry.ClassName == exportObj.TableEntry.ClassName)
                            {
                                isRepeat = true;
                                break;
                            }
                        }

                        if (isRepeat)
                        {
                            repeatObjects++;
                            continue;
                        }
                    }

                    numObjects++;
                    ObjectsByUncookedArchiveName[topPackage].Add(fullObjectPath, exportObj);
                    UncookedArchiveNameByObjectPath[fullObjectPath] = topPackage;
                }
            }

            Log.Info($"Assigned {numObjects} objects across {ObjectsByUncookedArchiveName.Count} packages.");
            Log.Info($"Skipped {skippedArchives} archives, {skippedObjects} export objects, and {repeatObjects} seemingly-repeated objects.");

            Log.Info("Copying data into uncooked archives..");
            Log.EmptyLine();

            OutputArchives = new FArchive[ObjectsByUncookedArchiveName.Count];
            int outArchiveIndex = 0;
            int totalNumObjects = numObjects;
            numObjects = 0;

            Window progressBarWindow = new Window(Console.WindowWidth, 1);
            ProgressBar progressBar = new ProgressBar(progressBarWindow, PbStyle.SingleLine, ObjectsByUncookedArchiveName.Count);

            // Get rid of any stored progress bars because they won't be scoped to our local window
            Window localProgressBarWindow = new Window(Console.WindowWidth, 10, Console.ForegroundColor, Console.BackgroundColor);
            ParallelProgressBars.Clear();

            Parallel.ForEach(ObjectsByUncookedArchiveName, new ParallelOptions { MaxDegreeOfParallelism = 100 }, (entry) =>
            {
                string fileName = entry.Key;
                MultiValueDictionary<string, UObject> objectsByName = entry.Value;

                if (NeverUncook.Contains(fileName))
                {
                    return;
                }

                FArchive outArchive = new FArchive(entry.Key, this);

                // Add a few intrinsics that won't be populated naturally
                outArchive.AddImportObject("Core", "Package", 0, "Core");
                outArchive.AddImportObject("Core", "Package", 0, "Engine");
                outArchive.AddImportObject("Core", "Class", -1, "Enum");

                // Every archive will need None to mark the end of tagged properties, but unless the name None is explicitly
                // used in an object, it won't get added to the archive's name table until after it's already been serialized to
                // disk, causing None to be unfindable when loading the UPK later. So we just manually kickstart it here
                outArchive.GetOrCreateName("None");

                lock (this)
                {
                    OutputArchives[outArchiveIndex] = outArchive;
                    progressBar.Refresh(Interlocked.Increment(ref outArchiveIndex), "Archives uncooking");
                }

                // TODO: the archive should be managing this state internally
                int exportCount = objectsByName.CountWhere(obj => obj is not UPackage || obj.ObjectName != outArchive.FileName);
                outArchive.ExportedObjects = new UObject[exportCount];

                if (UncookOnly.Count > 0 && !UncookOnly.Contains(outArchive.FileName))
                {
                    return;
                }

                var localProgressBar = GetOrCreateProgressBar(exportCount, localProgressBarWindow);
                int numObjectsExported = 0;
                float refreshTarget = exportCount / 10.0f;

                foreach (var subentry in objectsByName)
                {
                    List<UObject> objects = subentry.Value;

                    foreach (var obj in objects)
                    {
                        numObjectsExported++;

                        outArchive.AddExportObject(obj);
                    }

                    if (numObjectsExported >= refreshTarget)
                    {
                        localProgressBar.Refresh(numObjectsExported, outArchive.FileName);
                        refreshTarget += exportCount / 10.0f;
                    }
                }

                localProgressBar.Refresh(exportCount, outArchive.FileName);
                ParallelProgressBars.Enqueue(localProgressBar); // return to the pool
            });

            progressBar.Refresh(ObjectsByUncookedArchiveName.Count, "Archives uncooking");
            Log.Info($"Done creating uncooked archives in memory. Created {OutputArchives.Length} archives, with a total of {numObjects} objects exported from them.");

            // Give objects in the output archives a chance to post-process if needed
            foreach (var archive in OutputArchives)
            {
                if (archive == null)
                {
                    continue;
                }

                if (NeverUncook.Contains(archive.FileName))
                {
                    continue;
                }

                if (UncookOnly.Count > 0 && !UncookOnly.Contains(archive.FileName))
                {
                    continue;
                }

                foreach (var obj in archive.ExportedObjects)
                {
                    obj.PostArchiveCloneComplete();
                }
            }

            OutputArchives = OutputArchives.Where(ShouldUncook).ToArray();
            Log.Info($"After filtering, will serialize {OutputArchives.Length} archives to disk");

            int numArchivesCompleted = 0;
            progressBar = new ProgressBar(PbStyle.DoubleLine, OutputArchives.Length);

            foreach (var archive in OutputArchives)
            {
                progressBar.Refresh(Interlocked.Increment(ref numArchivesCompleted), archive.FileName);

                // Some archives cause issues in the UDK; still uncook them, but remap their names so they don't get loaded.
                // This has to be done after adding their export objects, or they won't be found in the export object
                // map due to archive name mismatch.
                switch (archive.FileName)
                {
                    // Pre-existing engine UPKs
                    case "EditorResources":
                    case "EngineDebugMaterials":
                    case "EngineFonts":
                    case "EngineMaterials":
                    case "EngineMeshes":
                    case "EngineResources":
                    case "EngineSounds":
                    case "EngineVolumetrics":
                    case "Engine_MaterialFunctions02":
                    case "Engine_MI_Shaders":

                    // UPKs with known issues that prevent loading
                    case "PhysicalMaterials":
                        archive.FileName += "_disabled_by_uncooker";
                        break;
                }

                PackageOrganizer.TryMatchPackageToFolders(archive, out string archiveFolder);
                string archiveFolderPath = Path.Combine(outputPath, archiveFolder);
                string extension = archive.IsMap ? ".udk" : ".upk";
                string archivePath = Path.Combine(archiveFolderPath, archive.FileName + extension);

                Directory.CreateDirectory(archiveFolderPath);

                var stream = new UnrealDataWriter(File.Open(archivePath, FileMode.Create));

                archive.BeginSerialization(stream);
                archive.SerializeHeaderData();

                archive.SerializeBodyData(progressBar);

                // The first time we serialize the header, we don't know all of the sizes/offsets that we need;
                // so once the body is serialized, we go back and do the header again.
                stream.Seek(0, SeekOrigin.Begin);
                archive.SerializeHeaderData();

                archive.EndSerialization();
            }
        }

        private ProgressBar GetOrCreateProgressBar(int max, Window window = null)
        {
            if (ParallelProgressBars.TryDequeue(out var progressBar))
            {
                progressBar.Max = max;
                progressBar.Refresh(0, "");
                return progressBar;
            }

            progressBar = window != null ? new ProgressBar(window, PbStyle.SingleLine, max) : new ProgressBar(PbStyle.SingleLine, max);
            return progressBar;
        }

        private byte[] ReadStreamData(FileStream stream, int startPosition, int numBytes)
        {
            byte[] buffer = new byte[numBytes];

            lock (stream)
            {
                stream.Seek(startPosition, SeekOrigin.Begin);
                stream.Read(buffer, 0, numBytes);

                return buffer;
            }
        }

        private bool ShouldUncook(FArchive archive)
        {
            // Some archives may be null because they're in the NeverUncook set; just skip
            if (archive == null)
            {
                return false;
            }

            // Occasionally an uncooked archive is empty, because it only consisted of things which don't exist in an uncooked
            // archive - for example, if its name was used as a package grouping but never contained anything except for
            // other packages. Just skip those archives.
            if (archive.ExportedObjects.Length == 0)
            {
                return false;
            }

            // There's a few archives that we do not want to uncook, because they'll conflict with their
            // compiled-script equivalents. Uncooked, they shouldn't contain anything but class data anyway
            if (NeverUncook.Contains(archive.FileName))
            {
                return false;
            }

            if (UncookOnly.Count > 0 && !UncookOnly.Contains(archive.FileName))
            {
                return false;
            }

            return true;
        }
    }
}
