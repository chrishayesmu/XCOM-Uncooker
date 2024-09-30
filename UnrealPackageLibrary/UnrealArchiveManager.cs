using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal;
using UnrealArchiveLibrary.Unreal.Intrinsic.Core;
using UnrealArchiveLibrary.Utils;

namespace UnrealPackageLibrary
{
    public class UnrealArchiveManager : IUnrealArchiveManager
    {
        private ArchiveManagerSettings _settings = new();
        public ArchiveManagerSettings Settings { get => _settings; }

        private Linker _inputLinker;
        public Linker InputLinker { get => _inputLinker; }

        private readonly ILogger _logger;

        private DirectoryInfo? tempDecompressionDirectory = null;

        public UnrealArchiveManager(ILoggerFactory? loggerFactory)
        {
            _logger = loggerFactory?.CreateLogger("UnrealArchiveLibrary") ?? NullLoggerFactory.Instance.CreateLogger("UnrealArchiveLibrary");
            _inputLinker = new Linker(_logger);
        }

        public Linker CloneArchives(IEnumerable<FArchive> archives, Linker? linkerToUse = null)
        {
            throw new NotImplementedException();
        }

        public ISet<string> GetLogicalArchivesFromInputs(IEnumerable<FArchive>? archives = null)
        {
            var logicalArchives = new HashSet<string>();

            var inputArchives = archives ?? InputLinker.Archives;

            foreach (var archive in inputArchives)
            {
                foreach (var exportEntry in archive.ExportTable)
                {
                    if (exportEntry.IsPackage && exportEntry.OuterIndex == 0)
                    {
                        logicalArchives.Add(exportEntry.ObjectName);
                    }
                }
            }

            return logicalArchives;
        }

        public void LoadInputArchives(IEnumerable<string> inputPackagePaths)
        {
            LoadInputArchives("", inputPackagePaths, DependencyLoadingMode.None);
        }

        public void LoadInputArchives(string baseDirectory, IEnumerable<string> inputPackagePaths, DependencyLoadingMode dependencyMode = DependencyLoadingMode.All)
        {
            _logger.LogDebug("LoadInputArchives: inputPackagePaths.Count = {numInputPackagePaths}, dependencyMode = {dependencyMode},  baseDirectory = {baseDirectory}", inputPackagePaths.Count(), dependencyMode, baseDirectory);

            #region Validate inputs

            if (dependencyMode != DependencyLoadingMode.None && !Directory.Exists(baseDirectory))
            {
                throw new DirectoryNotFoundException($"LoadInputArchives: dependencyMode is {dependencyMode}, but the directory \"{baseDirectory}\" could not be found");
            }

            foreach (string path in inputPackagePaths)
            {
                if (!File.Exists(path))
                {
                    throw new FileNotFoundException($"Input file {path} could not be found");
                }
            }

            #endregion

            var archiveToPathMap = new Dictionary<string, string>();
            
            #region Enumerate any potential archive files for dependency resolution

            if (dependencyMode != DependencyLoadingMode.None)
            {
                var allFiles = Directory.GetFiles(baseDirectory, "*", SearchOption.AllDirectories);
                var archiveFiles = allFiles.Where(file => Settings.ArchiveFileExtensions.Contains(Path.GetExtension(file)));

                archiveToPathMap = archiveFiles.ToDictionary(
                    file => Path.GetFileNameWithoutExtension(file).Replace("_SF", ""), // key: normalized archive name
                    file => file); // value: path to the file
            }

            #endregion

            var allAddedArchives = new List<FArchive>(); // everything loaded successfully during this function
            var latestAddedArchives = new ConcurrentQueue<FArchive>(); // everything just loaded in the latest iteration of dependency management
            var pendingArchives = new List<FArchive>(); // everything that hasn't been deserialized at all yet

            #region Initialize archives

            foreach (string path in inputPackagePaths)
            {
                string archiveName = Path.GetFileNameWithoutExtension(path);

                if (InputLinker.HasArchiveWithFileName(archiveName))
                {
                    _logger.LogWarning("The archive {archiveName} has already been loaded and will be skipped", archiveName);
                    continue;
                }

                var archive = OpenArchiveForRead(path, archiveName);
                pendingArchives.Add(archive);
            }

            #endregion

            // Loop through the archives, adding their dependencies (if requested) and decompressing any which need it
            while (pendingArchives.Count > 0)
            {
                var compressedArchives = new ConcurrentQueue<FArchive>();

                #region Read archive headers

                Parallel.ForEach(pendingArchives, (archive) =>
                {
                    try
                    {
                        archive.SerializeHeaderData();

                        if (archive.IsBodyCompressed || archive.IsFullyCompressed)
                        {
                            compressedArchives.Enqueue(archive);
                        }
                        else
                        {
                            latestAddedArchives.Enqueue(archive);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Could not initialize archive {fileName} - it will be skipped.", archive.FileName);
                    }
                });

                #endregion

                allAddedArchives.AddRange(latestAddedArchives);
                InputLinker.Archives.AddRange(latestAddedArchives);
                latestAddedArchives.Clear();
                pendingArchives.Clear();

                #region Handle compressed files

                if (!compressedArchives.IsEmpty)
                {
                    _logger.LogInformation("There are {numCompressedArchives} compressed archives in the active set. Decompressing them to a temporary location.", compressedArchives.Count);

                    var decompressedArchives = DecompressArchives(compressedArchives);

                    // Put these in pending so they can have their headers read
                    pendingArchives.AddRange(decompressedArchives);
                }

                #endregion

                #region Add unloaded dependencies if needed

                if (dependencyMode != DependencyLoadingMode.None)
                {
                    var allDependencies = new HashSet<string>();
                    var typeDependencies = new HashSet<string>();

                    foreach (var newArchive in latestAddedArchives)
                    {
                        foreach (var importEntry in newArchive.ImportTable)
                        {
                            // Top-level packages should always map to an archive file
                            if (importEntry.IsPackage && importEntry.OuterIndex == 0)
                            {
                                allDependencies.Add(importEntry.ObjectName);
                            }

                            // TODO: whenever there's a class import, track its outermost index and resolve it to a package later
                            // TODO: check dependencyMode in here
                        }
                    }

                    // Go through and strip out any that are already loaded in
                    allDependencies.RemoveWhere(dep => InputLinker.HasArchiveWithNormalizedName(dep));

                    _logger.LogInformation("Found {numDependencies} archives to load as dependencies", allDependencies.Count);

                    // Check if any of the dependencies couldn't be located
                    var missingDependencies = allDependencies.Where(archiveName => !archiveToPathMap.ContainsKey(archiveName));

                    foreach (var missingEntry in missingDependencies)
                    {
                        _logger.LogWarning("Archive '{archiveName}' is a dependency, but it couldn't be located", missingEntry);
                    }

                    // For archives that do have corresponding files, load them into archives
                    foreach (var dep in allDependencies)
                    {
                        if (!archiveToPathMap.ContainsKey(dep))
                        {
                            continue;
                        }

                        var archive = OpenArchiveForRead(archiveToPathMap[dep], dep);
                        pendingArchives.Add(archive);
                    }
                }

                #endregion
            }

            // All dependencies are now loaded (or can't be found); deserialize each archive's class data next,
            // so it's available for other archives to access in the next phase

            #region Deserialize exported classes

            _logger.LogInformation("Beginning deserialization of exported classes for {numArchives} archives..", allAddedArchives.Count);
            int serializedArchives = 0;

            Parallel.ForEach(allAddedArchives, new ParallelOptions() { MaxDegreeOfParallelism = Settings.MaxParallelismForSerialization },(archive) =>
            {
                _logger.LogDebug("Serializing exports for archive {Index}", Interlocked.Increment(ref serializedArchives));
                archive.SerializeClassExports();
            });

            #endregion

            // With the class data available, we can move on to deserializing all export objects
            Parallel.ForEach(allAddedArchives, (archive) =>
            {
                archive.SerializeBodyData();
            });

            foreach (var archive in allAddedArchives)
            {
                archive.EndSerialization();
            }
        }

        public Linker UncookArchives(IEnumerable<TextureFileCacheEntry>? textureFileCacheEntries = null, IEnumerable<FArchive>? inputArchivesOverride = null, IEnumerable<string>? outputArchivesOverride = null)
        {
            var uncookedLinker = new Linker(_logger);
            var inputArchives = inputArchivesOverride ?? InputLinker.Archives;

            var inputPackagesByGuid = new Dictionary<Guid, UPackage>();

            if (textureFileCacheEntries != null)
            {
                foreach (var entry in textureFileCacheEntries)
                {
                    uncookedLinker.RegisterTextureFileCache(entry.TextureFileName, entry.FilePath);
                }
            }

            // Start by finding all of the top-level packages that will exist in the uncooked data set
            var allPackages = new HashSet<string>() { "Core", "Engine" };
            var packageGuids = new Dictionary<Guid, Dictionary<string, string>>();

            foreach (var archive in inputArchives)
            {
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
                        inputPackagesByGuid[p.ExportTableEntry.PackageGuid] = p;
                    }

                    if (!packageGuids.ContainsKey(p.ExportTableEntry.PackageGuid))
                    {
                        packageGuids[p.ExportTableEntry.PackageGuid] = new Dictionary<string, string>();
                    }

                    packageGuids[p.ExportTableEntry.PackageGuid][p.NormalizedName] = p.ObjectName;
                });
            }

            // Initialize storage for each of these, arranged by package
            var objectsByUncookedArchiveName = new Dictionary<string, MultiValueDictionary<string, UObject>>();
            var uncookedArchiveNameByObjectPath = new Dictionary<string, string>();

            foreach (var package in allPackages)
            {
                objectsByUncookedArchiveName.Add(package, new MultiValueDictionary<string, UObject>());
            }

            // Now iterate every package's exports, assigning them to their original source archive
            int numObjects = 0, skippedObjects = 0, repeatObjects = 0;
            foreach (var archive in inputArchives)
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
                    if (!objectsByUncookedArchiveName.ContainsKey(topPackage))
                    {
                        skippedObjects++;
                        continue;
                    }

                    // If this object already exists, it must've been exported by another archive also, in which
                    // case we assume the objects are identical and just use whichever one got there first
                    if (objectsByUncookedArchiveName[topPackage].TryGetValue(fullObjectPath, out var objectsWithSamePath))
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
                    objectsByUncookedArchiveName[topPackage].Add(fullObjectPath, exportObj);
                    uncookedArchiveNameByObjectPath[fullObjectPath] = topPackage;
                }
            }

            var outputArchives = new FArchive[objectsByUncookedArchiveName.Count];
            int outArchiveIndex = 0;
            int totalNumObjects = numObjects;
            numObjects = 0;

            Parallel.ForEach(objectsByUncookedArchiveName, new ParallelOptions { MaxDegreeOfParallelism = Settings.MaxParallelismForUncooking }, (entry) =>
            {
                string fileName = entry.Key;
                MultiValueDictionary<string, UObject> objectsByName = entry.Value;

                if (outputArchivesOverride != null && !outputArchivesOverride.Contains(fileName))
                {
                    return;
                }

                FArchive outArchive = new FArchive(entry.Key, uncookedLinker, _logger);

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
                    outputArchives[outArchiveIndex] = outArchive;
                    Interlocked.Increment(ref outArchiveIndex);
                }

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

            return uncookedLinker;
        }

        private IEnumerable<FArchive> DecompressArchives(IEnumerable<FArchive> archivesToDecompress)
        {
            var decompressedArchives = new ConcurrentQueue<FArchive>();

            // Create decompressed copies on disk in a temp location
            tempDecompressionDirectory = Directory.CreateTempSubdirectory("XComUncooker_");
            _logger.LogInformation("Temp directory: {TempDirectory}", tempDecompressionDirectory);

            Parallel.ForEach(archivesToDecompress, new ParallelOptions() { MaxDegreeOfParallelism = Settings.MaxParallelismForDecompression }, (compressedArchive) =>
            {
                // TODO: instead of .upk, this should use the original extension
                string archivePath = Path.Combine(tempDecompressionDirectory.FullName, compressedArchive.FileName + ".upk");

                using (UnrealDataWriter tempFileStream = new UnrealDataWriter(File.Open(archivePath, FileMode.Create)))
                {
                    compressedArchive.DecompressToStream(tempFileStream);
                    compressedArchive.EndSerialization();
                }

                // Create a new archive and catch it up to where any previous archives are. Replace the previous stream
                // with a read-only one, because having a write stream open will interfere with cleaning up the temp directory.
                var decompressedArchive = OpenArchiveForRead(archivePath, compressedArchive.FileName);
                decompressedArchives.Enqueue(decompressedArchive);
            });

            return decompressedArchives;
        }

        private FArchive OpenArchiveForRead(string archivePath, string archiveName)
        {
            var fileStream = File.Open(archivePath, FileMode.Open, FileAccess.Read);
            var archive = new FArchive(archiveName, InputLinker, _logger);
            archive.BeginSerialization(new UnrealDataReader(fileStream));

            return archive;
        }

        public void Dispose()
        {
            InputLinker?.Dispose();

            // Try to clean up the temp directory, because it's probably full of decompressed
            // versions of UPK files, meaning it's several gigabytes in size
            if (tempDecompressionDirectory != null)
            {
                _logger.LogInformation("Deleting temp directory {TempDirectory}..", tempDecompressionDirectory);

                try
                {
                    Directory.Delete(tempDecompressionDirectory.FullName, true);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Error occurred while deleting temp directory. Manual cleanup will be required.");
                }

                tempDecompressionDirectory = null;
            }
        }
    }
}
