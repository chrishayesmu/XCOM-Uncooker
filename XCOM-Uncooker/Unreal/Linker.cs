using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.Unreal.Physical;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core;

namespace XCOM_Uncooker.Unreal
{
    public class Linker
    {
        public static readonly List<string> FilesToSkip = ["Act1_IntroLevel.upk", "Act1_IntroLevel_Anim.upk", "Act1_IntroLevel_Anim_LOC_INT.upk", "Act1_IntroLevel_Script.upk", "Act1_IntroLevel_Script_LOC_INT.upk"];

        public FArchive[] InputArchives;
        public FArchive[] OutputArchives;

        public IDictionary<string, IDictionary<string, UObject>> ObjectsByUncookedArchiveName;
        public IDictionary<string, string> UncookedArchiveNameByObjectPath;
        public IDictionary<Guid, UPackage> InputPackagesByGuid;

        private bool UseVerboseLogging;

        public Linker(bool logVerbose)
        {
            UseVerboseLogging = logVerbose;
        }

        public void LoadArchives(List<string> filePaths)
        {
            var validPaths = filePaths.Where(path => Path.Exists(path) && !FilesToSkip.Contains(Path.GetFileName(path))).ToList();

            int fileColumnSize = 3 + Math.Max(14, filePaths.Max(path => Path.GetFileNameWithoutExtension(path).Length));
            string columnSpecs = "{0," + fileColumnSize + "} | {1,15} | {2,15} | {3,15} | {4,15}";

            if (UseVerboseLogging)
            {
                Console.WriteLine(columnSpecs, "Archive file", "Name count", "Import count", "Export count", "DependsMap?");
                Console.WriteLine("-------------------------------------------------------------------------------------------------------------");
            }

            InputArchives = new FArchive[validPaths.Count];

            int numSucceeded = 0, numFailed = 0;

            Console.WriteLine($"Attempting to read archive headers for {InputArchives.Length} archives..");
            for (int i = 0; i < validPaths.Count; i++)
            {
                try
                {
                    var stream = File.Open(validPaths[i], FileMode.Open);
                    InputArchives[i] = new FArchive(Path.GetFileNameWithoutExtension(validPaths[i]), this);
                    InputArchives[i].BeginSerialization(stream);
                    InputArchives[i].SerializeHeaderData();
                    numSucceeded++;

                    if (UseVerboseLogging)
                    {
                        Console.WriteLine(columnSpecs, InputArchives[i].FileName, InputArchives[i].NameTable.Count, InputArchives[i].ImportTable.Count, InputArchives[i].ExportTable.Count, InputArchives[i].HasDependsMap);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR: could not initialize archive at \"{validPaths[i]}\" - it will be skipped.");
                    Console.WriteLine(ex.ToString());
                    numFailed++;
                }
            }

            Console.WriteLine($"Linker: Done parsing archive headers. {numSucceeded} succeeded and {numFailed} failed.");
            Console.WriteLine("Beginning deserialization of exported classes for archives..");
            
            foreach (var archive in InputArchives)
            {
                if (archive == null)
                {
                    continue;
                }

                archive.SerializeClassExports();
            }

            Console.WriteLine("Deserialization of exported classes is complete.");
            Console.WriteLine("Beginning deserialization of export objects for archives..");

            foreach (var archive in InputArchives)
            {
                if (archive == null)
                {
                    continue;
                }

                archive.SerializeBodyData();
                archive.EndSerialization();
            }

            Console.WriteLine("Deserialization of export objects is complete.");
        }

        /// <summary>
        /// Finds the object specified by the given path, as long as it exists in at least one of the currently-loaded archives.
        /// </summary>
        /// <param name="fullObjectPath">The full object path, as from <see cref="FObjectTableEntry.FullObjectPath"/>.</param>
        /// <returns>The corresponding <see cref="UObject"/> if found, else null.</returns>
        /// <remarks>
        /// The return value of this is ambiguous if there are multiple objects with the same path. UE3 itself seems to
        /// have the same problem, and explicitly recommends against naming two objects with the same path. For uncooking, this
        /// will occur naturally because the same logical object has been pulled into multiple archives; we simply assume that
        /// all copies of the object are the same and use the first match we find.
        /// </remarks>
        public UObject GetCookedObjectByPath(string fullObjectPath)
        {
            foreach (var archive in InputArchives)
            {
                var obj = archive.GetExportedObjectByPath(fullObjectPath);

                if (obj != null)
                {
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
                if (importEntry.IsPackage)
                {
                    return importEntry.ObjectName;
                }
                else
                {
                    // shouldn't be possible? can a top level import point to anything other than a package?
                    Debugger.Break();
                }
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
                // should only happen for maps, and maybe weird situations in Core/Engine (I think?)
                return exportEntry.Archive.NormalizedName;
            }

            return "UNKNOWN";
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

        public void UncookArchives()
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

                if (archive.IsMap)
                {
                    allPackages.Add(archive.NormalizedName);
                }

                archive?.TopLevelPackages.ForEach(p => { 
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

            Console.WriteLine($"Found {allPackages.Count} distinct top level packages.");

            // Initialize storage for each of these, arranged by package
            ObjectsByUncookedArchiveName = new Dictionary<string, IDictionary<string, UObject>>();
            UncookedArchiveNameByObjectPath = new Dictionary<string, string>();

            foreach (var package in allPackages)
            {
                ObjectsByUncookedArchiveName.Add(package, new Dictionary<string, UObject>());
            }

            // Now iterate every package's exports, assigning them to their original source archive
            int skippedArchives = 0, skippedObjects = 0, repeatObjects = 0;
            foreach (var archive in InputArchives)
            {
                if (archive == null)
                {
                    continue;
                }

                // TODO: need a way to remove the dependencies from map packages and only leave the level stuff
                if (archive.IsMap)
                {
                    //skippedArchives++;
                    //continue;
                }

                foreach (var exportObj in archive.ExportedObjects)
                {
                    string fullObjectPath = exportObj.FullObjectPath;
                    string topPackage = fullObjectPath.Split(".")[0];

                    // TODO: handle map objects better
                    if (topPackage == "TheWorld")
                    {
                        topPackage = archive.NormalizedName;
                    }

                    if (!ObjectsByUncookedArchiveName.ContainsKey(topPackage))
                    {
                        //ObjectsByUncookedArchiveName.Add(topPackage, new Dictionary<string, UObject>());
                        skippedObjects++;
                        continue;
                    }

                    // If this object already exists, it must've been exported by another archive also, in which
                    // case we assume the objects are identical and just use whichever one got there first
                    if (ObjectsByUncookedArchiveName[topPackage].ContainsKey(fullObjectPath))
                    {
                        repeatObjects++;
                        continue;
                    }

                    ObjectsByUncookedArchiveName[topPackage].Add(fullObjectPath, exportObj);
                    UncookedArchiveNameByObjectPath[fullObjectPath] = topPackage;
                }
            }

            int total = 0;
            using (var writer = new StreamWriter("objects-by-archive.csv"))
            {
                writer.WriteLine("Archive,Object path");
                //Console.WriteLine("{0,35}{1,15}", "Archive", "Object count");
                foreach (var entry in ObjectsByUncookedArchiveName)
                {
                    foreach (var subentry in entry.Value)
                    {
                        writer.WriteLine($"{entry.Key},{subentry.Key}");
                    }
                    // outputData += $"{entry.Key},{entry.Value.Count}\n";
                    total += entry.Value.Count;
                    //Console.WriteLine("{0,35}{1,15}", entry.Key, entry.Value.Count);
                }
            }

            //File.WriteAllText("object-count-by-archive.csv", outputData);

            Console.WriteLine($"Assigned {total} objects across {ObjectsByUncookedArchiveName.Count} packages.");
            Console.WriteLine($"Skipped {skippedArchives} archives, {skippedObjects} export objects, and {repeatObjects} seemingly-repeated objects.");

            var unmatchedPackages = allPackages.Where(p => !PackageOrganizer.TryMatchPackageToFolders(p, out _));
            Console.WriteLine($"There are {unmatchedPackages.Count()} packages which were not matched to a folder structure.");

            File.WriteAllText("allPackages.txt", string.Join('\n', allPackages.Order()));
            File.WriteAllText("unmatchedPackages.txt", string.Join('\n', unmatchedPackages.Order()));


            Console.WriteLine("Copying data into uncooked archives..");

            OutputArchives = new FArchive[ObjectsByUncookedArchiveName.Count];
            int i = 0, numObjects = 0;

            foreach (var entry in ObjectsByUncookedArchiveName)
            {
                FArchive outArchive = new FArchive(entry.Key, this);
                outArchive.AddImportObject("Core", "Package", 0, "Core");
                outArchive.AddImportObject("Core", "Package", 0, "Engine");
                outArchive.AddImportObject("Core", "Class", 0, "Enum");

                OutputArchives[i++] = outArchive;

                foreach (var subentry in entry.Value)
                {
                    UObject obj = subentry.Value;

                    //Console.WriteLine($"Archive {outArchive.FileName}: trying to add object {obj.FullObjectPath}");
                    outArchive.AddExportObject(obj);
                    numObjects++;

                    if (numObjects % 250 == 0)
                    {
                        Console.WriteLine($"    {numObjects} export objects added to uncooked archives..");

                        if (numObjects == 50000 || numObjects == 80000)
                        {
                            Debugger.Break();
                        }
                    }
                }
            }

            //UObject obj = GetCookedObjectByPath("CHH_FacialHair.Textures.FacialHairTile");
            //
            //Console.WriteLine($"Archive {outArchive.FileName}: trying to add object {obj.FullObjectPath}");
            //outArchive.AddExportObject(obj);

            Console.WriteLine($"Done creating uncooked archives in memory. Created {OutputArchives.Length} archives, with a total of {numObjects} objects exported from them.");

            // Console.WriteLine($"After adding: archive has {outArchive.NameTable.Count} names, {outArchive.ExportTable.Count} exports, and {outArchive.ImportTable.Count} imports");
        }
    }
}
