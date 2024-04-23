﻿using System;
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

namespace XCOM_Uncooker.Unreal
{
    public class Linker
    {
        private static readonly Logger Log = new Logger(nameof(Linker));

        public static readonly List<string> FilesToSkip = []; // ["Act1_IntroLevel.upk", "Act1_IntroLevel_Anim.upk", "Act1_IntroLevel_Anim_LOC_INT.upk", "Act1_IntroLevel_Script.upk", "Act1_IntroLevel_Script_LOC_INT.upk"];

        public FArchive[] InputArchives;
        public FArchive[] OutputArchives;

        public IDictionary<string, IDictionary<string, UObject>> ObjectsByUncookedArchiveName;
        public IDictionary<string, string> UncookedArchiveNameByObjectPath;
        public IDictionary<Guid, UPackage> InputPackagesByGuid;

        private IDictionary<string, FileStream> TextureFileCaches = new Dictionary<string, FileStream>();
        private ConcurrentQueue<ProgressBar> ParallelProgressBars = [];

        public void LoadArchives(List<string> filePaths)
        {
            var validPaths = filePaths.Where(path => Path.Exists(path) && !FilesToSkip.Contains(Path.GetFileName(path))).ToList();

            InputArchives = new FArchive[validPaths.Count];

            int numArchivesCompleted = 0, numSucceeded = 0, numFailed = 0;

            #region Deserialize archive headers

            Log.Info($"Attempting to read archive headers for {InputArchives.Length} archives..");

            ProgressBar headerProgressBar = new ProgressBar("Reading headers");
            Log.DisplayProgressBar(headerProgressBar);

            // Create the archives before the parallelization, makes the parallel bit simpler
            for (int i = 0; i < validPaths.Count; i++) 
            {
                var stream = new UnrealDataReader( File.Open(validPaths[i], FileMode.Open, FileAccess.Read));
                InputArchives[i] = new FArchive(Path.GetFileNameWithoutExtension(validPaths[i]), this);
                InputArchives[i].BeginSerialization(stream);
            }

            numArchivesCompleted = 0;

            Parallel.ForEach(InputArchives, (archive) =>
            {
                try
                {
                    archive.SerializeHeaderData();
                    Interlocked.Increment(ref numSucceeded);
                    headerProgressBar.Update("", Interlocked.Increment(ref numArchivesCompleted), InputArchives.Length);

                }
                catch (Exception ex)
                {
                    Log.Error($"Could not initialize archive \"{archive.FileName}\" - it will be skipped.");
                    Log.Info(ex.ToString());
                    Interlocked.Increment(ref numFailed);
                }
            });

            headerProgressBar.Update("complete", InputArchives.Length, InputArchives.Length);
            Log.RemoveProgressBar(headerProgressBar);
            Log.Info($"Done parsing archive headers. {numSucceeded} succeeded and {numFailed} failed.");

            #endregion

            #region Deserialize exported classes

            Log.Info("Beginning deserialization of exported classes for archives..");

            ProgressBar classDeserializationProgressBar = new ProgressBar("Deserializing classes");
            Log.DisplayProgressBar(classDeserializationProgressBar);

            numArchivesCompleted = 0;

            Parallel.ForEach(InputArchives, (archive) =>
            {
                if (archive == null)
                {
                    return;
                }

                archive.SerializeClassExports();

                classDeserializationProgressBar.Update(archive.FileName, Interlocked.Increment(ref numArchivesCompleted), InputArchives.Length);
            });

            classDeserializationProgressBar.Update("complete", InputArchives.Length, InputArchives.Length);
            Log.RemoveProgressBar(classDeserializationProgressBar);
            Log.Info("Deserialization of exported classes is complete.");

            #endregion

            #region Deserialize export objects

            Log.Info("Beginning deserialization of export objects for archives..");

            ProgressBar exportDeserializationProgressBar = new ProgressBar("Deserializing main package data")
            {
                BackgroundColor = ConsoleColor.DarkBlue,
                ForegroundColor = ConsoleColor.White
            };
            
            Log.DisplayProgressBar(exportDeserializationProgressBar);
            exportDeserializationProgressBar.Update("Packages processed", 0, InputArchives.Length);

            numArchivesCompleted = 0;

            Parallel.ForEach(InputArchives, (archive) =>
            {
                if (archive == null) 
                { 
                    return; 
                }

                var progressBar = GetOrCreateParallelProgressBar(archive.FileName);
                archive.SerializeBodyData(progressBar);

                ParallelProgressBars.Enqueue(progressBar); // put the bar back in the pool for re-use
                exportDeserializationProgressBar.Update("Packages processed", Interlocked.Increment(ref numArchivesCompleted), InputArchives.Length);
            });

            // End serialization after all archives are done, due to some threading issues I'm too lazy to fix
            foreach (var archive in InputArchives)
            {
                archive.EndSerialization();
            }

            exportDeserializationProgressBar.Update("Packages processed", InputArchives.Length, InputArchives.Length);

            while (ParallelProgressBars.TryDequeue(out ProgressBar progressBar))
            {
                Log.RemoveProgressBar(progressBar);
            }

            #endregion

            Log.RemoveProgressBar(exportDeserializationProgressBar);
            Log.Info("Deserialization of export objects is complete.");
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
                // should only happen for maps, classes, and maybe weird situations in Core/Engine (I think?)
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

            Log.Info($"Found {allPackages.Count} distinct top level packages.");

            // Initialize storage for each of these, arranged by package
            ObjectsByUncookedArchiveName = new Dictionary<string, IDictionary<string, UObject>>();
            UncookedArchiveNameByObjectPath = new Dictionary<string, string>();

            foreach (var package in allPackages)
            {
                ObjectsByUncookedArchiveName.Add(package, new Dictionary<string, UObject>());
            }

            // Now iterate every package's exports, assigning them to their original source archive
            int skippedArchives = 0, numObjects = 0, skippedObjects = 0, repeatObjects = 0;
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

                    if (exportObj is UPackage && exportObj.ObjectName == archive.FileName)
                    {
                        continue;
                    }

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

                    numObjects++;
                    ObjectsByUncookedArchiveName[topPackage].Add(fullObjectPath, exportObj);
                    UncookedArchiveNameByObjectPath[fullObjectPath] = topPackage;
                }
            }

            Log.Info($"Assigned {numObjects} objects across {ObjectsByUncookedArchiveName.Count} packages.");
            Log.Info($"Skipped {skippedArchives} archives, {skippedObjects} export objects, and {repeatObjects} seemingly-repeated objects.");

            string folderPath = "";
            var unmatchedPackages = allPackages.Where(p => !PackageOrganizer.TryMatchPackageToFolders(p, ref folderPath));
            Log.Info($"There are {unmatchedPackages.Count()} packages which were not matched to a folder structure.");

            File.WriteAllText("allPackages.txt", string.Join('\n', allPackages.Order()));
            File.WriteAllText("unmatchedPackages.txt", string.Join('\n', unmatchedPackages.Order()));


            Log.Info("Copying data into uncooked archives..");

            OutputArchives = new FArchive[ObjectsByUncookedArchiveName.Count];
            int i = 0;
            numObjects = 0;
            string output = "Archive\tObjects\tImports\n";

            Parallel.ForEach(ObjectsByUncookedArchiveName, new ParallelOptions { MaxDegreeOfParallelism = 100 }, (entry) =>
            {
                FArchive outArchive = new FArchive(entry.Key, this);

                // Add a few intrinsics that won't be populated naturally
                outArchive.AddImportObject("Core", "Package", 0, "Core");
                outArchive.AddImportObject("Core", "Package", 0, "Engine");
                outArchive.AddImportObject("Core", "Class", -1, "Enum");

                // Every archive will need None to mark the end of tagged properties, but unless the name None is explicitly
                // used in an object, it won't get added to the archive's name table until after it's already been serialized to
                // disk, causing None to be unfindable when loading the UPK later. So we just manually kickstart it here
                outArchive.GetOrCreateName("None");

                OutputArchives[i++] = outArchive;

                // TODO: the archive should be managing this state internally
                outArchive.ExportedObjects = new UObject[entry.Value.Count];

                foreach (var subentry in entry.Value)
                {
                    UObject obj = subentry.Value;

                    outArchive.AddExportObject(obj);
                    Interlocked.Increment(ref numObjects);

                    if (numObjects % 10000 == 0)
                    {
                        Log.Info($"{numObjects} export objects added to uncooked archives..");
                    }
                }

                output += $"{outArchive.FileName}\t{outArchive.ExportedObjects.Length}\t{outArchive.ImportTable.Count}\n";
            });

            File.WriteAllText("uncookedPackages.txt", output);
            Log.Info($"Done creating uncooked archives in memory. Created {OutputArchives.Length} archives, with a total of {numObjects} objects exported from them.");

            string[] archivesToWrite = ["GameData_Toughness_DLC"];

            foreach (var archive in OutputArchives)
            {
                Log.Info($"Attempting to write archive file {archive.FileName}");
                // var archive = OutputArchives.FirstOrDefault(ar => ar.FileName == archiveName);
                var stream = new UnrealDataWriter(File.Open($"archives/{archive.FileName}.upk", FileMode.Create));

                archive.BeginSerialization(stream);
                archive.SerializeHeaderData();

                var progressBar = GetOrCreateParallelProgressBar(archive.FileName);
                archive.SerializeBodyData(progressBar);

                // The first time we serialize the header, we don't know all of the sizes/offsets that we need;
                // so once the body is serialized, we go back and do the header again.
                stream.Seek(0, SeekOrigin.Begin);
                archive.SerializeHeaderData();

                archive.EndSerialization();

                ParallelProgressBars.Enqueue(progressBar); // put the bar back in the pool for re-use
            }

            while (ParallelProgressBars.TryDequeue(out ProgressBar progressBar))
            {
                Log.RemoveProgressBar(progressBar);
            }
        }

        private ProgressBar GetOrCreateParallelProgressBar(string title)
        {
            if (ParallelProgressBars.TryDequeue(out ProgressBar progressBar))
            {
                progressBar.Title = title;
                return progressBar;
            }

            progressBar = new ProgressBar(title);
            Log.DisplayProgressBar(progressBar);

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
    }
}
