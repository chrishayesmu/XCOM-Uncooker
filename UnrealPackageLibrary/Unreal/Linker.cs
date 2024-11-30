using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal;
using UnrealArchiveLibrary.Unreal.Intrinsic.Core;
using UnrealArchiveLibrary.Utils;

namespace UnrealArchiveLibrary.Unreal
{
    public class Linker(ILogger logger) : IDisposable
    {
        public List<FArchive> Archives = [];

        public IDictionary<string, MultiValueDictionary<string, UObject>> ObjectsByUncookedArchiveName = new Dictionary<string, MultiValueDictionary<string, UObject>>();
        public IDictionary<string, string> UncookedArchiveNameByObjectPath = new Dictionary<string, string>();
        public IDictionary<Guid, UPackage> InputPackagesByGuid = new Dictionary<Guid, UPackage>();

        private IDictionary<string, FileStream> TextureFileCaches = new Dictionary<string, FileStream>();

        public bool TryGetArchiveWithFileName(string name, out FArchive? archive)
        {
            archive = Archives.SingleOrDefault(ar => ar.FileName == name);

            return archive != null;
        }

        public bool TryGetArchiveWithNormalizedName(string name, out FArchive? archive)
        {
            archive = Archives.SingleOrDefault(ar => ar.NormalizedName == name);

            return archive != null;
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
            foreach (var archive in Archives)
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

            if (fullObjectPath.Contains('.'))
            {
                return fullObjectPath.Split('.')[0];
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

        /// <summary>
        /// Uncooks the given archives into this linker. Afterwards, the uncooked archives will be accessible via
        /// <see cref="Archives"/>. This is a destructive operation; <see cref="Archives"/> is completely replaced.
        /// </summary>
        /// <param name="archivesToUncook">Archives to use as source data; should be from a different Linker.</param>
        /// <param name="uncookedArchivesToKeep">If set, only archives with names in this list will be uncooked.</param>
        public void UncookArchives(IEnumerable<FArchive> archivesToUncook, IEnumerable<string>? uncookedArchivesToKeep)
        {
            // Start by finding all of the top-level packages that will exist in the uncooked data set
            var allPackages = new HashSet<string>() { "Core", "Engine" };
            var packageGuids = new Dictionary<Guid, Dictionary<string, string>>();

            foreach (var archive in archivesToUncook)
            {
                // TODO: this is probably not needed, leaving it in case it is and I forget about it
                allPackages.Add(archive.NormalizedName);

                if (archive.IsMap)
                {
                    allPackages.Add(archive.NormalizedName);
                }

                archive.TopLevelPackages.ForEach(p => { 
                    allPackages.Add(p.NormalizedName);

                    if (p.ExportTableEntry!.PackageGuid != Guid.Empty)
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

            foreach (var package in allPackages)
            {
                ObjectsByUncookedArchiveName.Add(package, new MultiValueDictionary<string, UObject>());
            }

            // Now iterate every package's exports, assigning them to their original source archive
            int numObjects = 0, skippedObjects = 0, repeatObjects = 0;
            foreach (var archive in Archives)
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

            var outputArchives = new FArchive[ObjectsByUncookedArchiveName.Count];
            int outArchiveIndex = -1;
            int totalNumObjects = numObjects;
            numObjects = 0;

            Parallel.ForEach(ObjectsByUncookedArchiveName, new ParallelOptions { MaxDegreeOfParallelism = 100 }, (entry) =>
            {
                string fileName = entry.Key;
                MultiValueDictionary<string, UObject> objectsByName = entry.Value;

                if (uncookedArchivesToKeep != null && !uncookedArchivesToKeep.Contains(fileName))
                {
                    return;
                }

                FArchive outArchive = new FArchive(entry.Key, this, logger);

                // Add a few intrinsics that won't be populated naturally
                outArchive.AddImportObject("Core", "Package", 0, "Core");
                outArchive.AddImportObject("Core", "Package", 0, "Engine");
                outArchive.AddImportObject("Core", "Class", -1, "Enum");

                // Every archive will need None to mark the end of tagged properties, but unless the name None is explicitly
                // used in an object, it won't get added to the archive's name table until after it's already been serialized to
                // disk, causing None to be unfindable when loading the UPK later. So we just manually kickstart it here
                outArchive.GetOrCreateName("None");

                outputArchives[Interlocked.Increment(ref outArchiveIndex)] = outArchive;

                // TODO: the archive should be managing this state internally
                int exportCount = objectsByName.CountWhere(obj => obj is not UPackage || obj.ObjectName != outArchive.FileName);
                outArchive.ExportedObjects = new UObject[exportCount];

                foreach (var subentry in objectsByName)
                {
                    List<UObject> objects = subentry.Value;

                    foreach (var obj in objects)
                    {
                        outArchive.AddExportObject(obj);
                    }
                }
            });

            // Give objects in the output archives a chance to post-process if needed
            foreach (var archive in outputArchives)
            {
                foreach (var obj in archive.ExportedObjects)
                {
                    obj.PostArchiveCloneComplete();
                }
            }

            // We don't need this info anymore, clear up the memory it's using
            InputPackagesByGuid.Clear();
            ObjectsByUncookedArchiveName.Clear();
            UncookedArchiveNameByObjectPath.Clear();

            Archives = outputArchives.ToList();
        }

        public void Dispose()
        {
            foreach (var archive in Archives)
            {
                archive?.EndSerialization();
            }
        }
    }
}
