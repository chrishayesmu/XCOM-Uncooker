using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.Graph;
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

            // Create decompressed copies on disk in a temp location
            tempDecompressionDirectory = Directory.CreateTempSubdirectory("XComUncooker_");
            _logger.LogInformation("Temp directory: {TempDirectory}", tempDecompressionDirectory);
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

        public void LoadInputArchives(IEnumerable<string> inputPackagePaths, ProgressHandler? progressHandler = null)
        {
            LoadInputArchives("", inputPackagePaths, progressHandler, DependencyLoadingMode.None);
        }

        public void LoadInputArchives(string baseDirectory, IEnumerable<string> inputPackagePaths, ProgressHandler? progressHandler = null, DependencyLoadingMode dependencyMode = DependencyLoadingMode.All)
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

            var headersLoadedArchives = new List<FArchive>();

            #region Initialize archives

            int numRequestedArchives = inputPackagePaths.Count();
            int numLoadedArchives = 0;
            progressHandler?.Invoke(ProgressEvent.ArchiveHeaderLoaded, 0, numRequestedArchives);

            foreach (string path in inputPackagePaths)
            {
                string archiveName = Path.GetFileNameWithoutExtension(path);

                if (InputLinker.TryGetArchiveWithFileName(archiveName, out _))
                {
                    _logger.LogWarning("The archive {archiveName} has already been loaded and will be skipped", archiveName);
                    progressHandler?.Invoke(ProgressEvent.ArchiveHeaderLoaded, ++numLoadedArchives, numRequestedArchives);
                    continue;
                }

                var archive = LoadSingleArchiveUpToHeaders(path);
                InputLinker.Archives.Add(archive);
                headersLoadedArchives.Add(archive);
                progressHandler?.Invoke(ProgressEvent.ArchiveHeaderLoaded, ++numLoadedArchives, numRequestedArchives);
            }

            #endregion

            if (dependencyMode != DependencyLoadingMode.None)
            {
                var unsatisfiableDependencies = new HashSet<string>(); // dependencies which were identified but could not be loaded
                var handledDependencies = new HashSet<string>(); // every dependency which is processed, either loaded or unsatisfiable
                var allDependencies = new HashSet<string>(); // every dependency identified
                var mostRecentDependencies = new HashSet<string>(); // dependencies identified in the latest iteration of dependency checking
                var mostRecentArchives = new List<FArchive>(headersLoadedArchives); // tracks only the archives loaded since the last iteration of dependency checking

                numLoadedArchives = 0;

                do
                {
                    // Identify the packages we depend on
                    foreach (var archive in mostRecentArchives)
                    {
                        foreach (var importEntry in archive.ImportTable)
                        {
                            // Top-level packages should always map to an archive file
                            if (importEntry.IsPackage && importEntry.OuterIndex == 0)
                            {
                                mostRecentDependencies.Add(importEntry.ObjectName);
                            }
                            else if (archive.IsMap && importEntry.IsClass && importEntry.OuterIndex > 0)
                            {
                                // Maps have a very weird behavior where they can have a cooked copy of a class in their exports, but at the same
                                // time, the class is still listed as an import. For our purposes, we need to load the original archive if we can
                                if (importEntry.OuterTable.IsPackage)
                                {
                                    mostRecentDependencies.Add(importEntry.OuterTable.ObjectName);
                                }
                            }

                            // TODO: check dependencyMode in here
                        }
                    }

                    allDependencies.UnionWith(mostRecentDependencies);
                    mostRecentArchives.Clear();

                    // Load as many of those packages as we can (that aren't already loaded)
                    foreach (var dep in mostRecentDependencies)
                    {
                        progressHandler?.Invoke(ProgressEvent.DependencyLoaded, handledDependencies.Count, allDependencies.Count);

                        if (InputLinker.TryGetArchiveWithNormalizedName(dep, out _))
                        {
                            handledDependencies.Add(dep);
                            continue;
                        }

                        if (archiveToPathMap.TryGetValue(dep, out string? depPath))
                        {
                            var archive = LoadSingleArchiveUpToHeaders(depPath);
                            headersLoadedArchives.Add(archive);
                            mostRecentArchives.Add(archive);
                            InputLinker.Archives.Add(archive);
                        }
                        else
                        {
                            unsatisfiableDependencies.Add(dep);
                        }

                        handledDependencies.Add(dep);
                    }

                    mostRecentDependencies.Clear();
                } while (mostRecentArchives.Count > 0);

                foreach (var dep in unsatisfiableDependencies)
                {
                    _logger.LogWarning("Couldn't load dependent archive {archiveName}", dep);
                }
            }

            // Build a dependency tree so we can operate on it breadth-first
            var dependencyTree = BuildDependencyTree(headersLoadedArchives);

            // All dependencies are now loaded (or can't be found); deserialize them in dependency order
            numLoadedArchives = 0;

            dependencyTree.TraverseBreadthFirstMultiple((archivesAtDepth, depth) =>
            {
                _logger.LogInformation("Processing {numArchives} at depth {depth}", archivesAtDepth.Count(), depth);

                Parallel.ForEach(archivesAtDepth, new ParallelOptions() { MaxDegreeOfParallelism = Settings.MaxParallelismForSerialization }, (archive) =>
                {
                    // Make sure the archive's open, in case we're carrying over previously-loaded archives
                    if (archive.IsOpen)
                    {
                        archive.SerializeBodyData();
                    }

                    progressHandler?.Invoke(ProgressEvent.ArchiveBodyLoaded, Interlocked.Increment(ref numLoadedArchives), dependencyTree.NodeCount);
                });
            });

            // Close all our file streams
            foreach (var archive in headersLoadedArchives)
            {
                archive.EndSerialization();
            }

            progressHandler?.Invoke(ProgressEvent.LoadComplete, headersLoadedArchives.Count, InputLinker.Archives.Count);
        }

        public Linker UncookArchives(IEnumerable<TextureFileCacheEntry>? textureFileCacheEntries = null, ProgressHandler? progressHandler = null, IEnumerable<FArchive>? inputArchivesOverride = null, ISet<string>? outputArchivesOverride = null)
        {
            var uncookedLinker = new Linker(_logger);
            var inputArchives = inputArchivesOverride ?? InputLinker.Archives;

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
                        uncookedLinker.InputPackagesByGuid[p.ExportTableEntry.PackageGuid] = p;
                    }

                    if (!packageGuids.ContainsKey(p.ExportTableEntry.PackageGuid))
                    {
                        packageGuids[p.ExportTableEntry.PackageGuid] = new Dictionary<string, string>();
                    }

                    packageGuids[p.ExportTableEntry.PackageGuid][p.NormalizedName] = p.ObjectName;
                });
            }

            // Initialize storage for each of these, arranged by package
            foreach (var package in allPackages)
            {
                if (outputArchivesOverride != null && !outputArchivesOverride.Contains(package))
                {
                    continue;
                }

                uncookedLinker.ObjectsByUncookedArchiveName.Add(package, new MultiValueDictionary<string, UObject>());
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
                    if (exportObj is UPackage && exportObj.ObjectName == archive.FileName)
                    {
                        continue;
                    }

                    string fullObjectPath = exportObj.FullObjectPath;
                    string topPackage = fullObjectPath.Split(".")[0];

                    if (outputArchivesOverride != null && !outputArchivesOverride.Contains(topPackage))
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
                    if (!uncookedLinker.ObjectsByUncookedArchiveName.ContainsKey(topPackage))
                    {
                        skippedObjects++;
                        continue;
                    }

                    // If this object already exists, it must've been exported by another archive also, in which
                    // case we assume the objects are identical and just use whichever one got there first
                    if (uncookedLinker.ObjectsByUncookedArchiveName[topPackage].TryGetValue(fullObjectPath, out var objectsWithSamePath))
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
                    uncookedLinker.ObjectsByUncookedArchiveName[topPackage].Add(fullObjectPath, exportObj);
                    uncookedLinker.UncookedArchiveNameByObjectPath[fullObjectPath] = topPackage;
                }
            }

            var outputArchives = new FArchive[uncookedLinker.ObjectsByUncookedArchiveName.Count];
            int outArchiveIndex = -1;
            int totalNumObjects = numObjects;
            int numArchivesProcessed = 0;
            numObjects = 0;

            Parallel.ForEach(uncookedLinker.ObjectsByUncookedArchiveName, new ParallelOptions { MaxDegreeOfParallelism = Settings.MaxParallelismForUncooking }, (entry) =>
            {
                string fileName = entry.Key;
                MultiValueDictionary<string, UObject> objectsByName = entry.Value;

                FArchive outArchive = new FArchive(entry.Key, uncookedLinker, _logger);

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

                progressHandler?.Invoke(ProgressEvent.ArchiveUncookedInMemory, Interlocked.Increment(ref numArchivesProcessed), outputArchives.Length);
            });

            uncookedLinker.Archives = outputArchives.ToList();

            // Give objects in the output archives a chance to post-process if needed
            numArchivesProcessed = 0;
            foreach (var archive in uncookedLinker.Archives)
            {
                foreach (var obj in archive.ExportedObjects)
                {
                    obj.PostArchiveCloneComplete();
                }

                progressHandler?.Invoke(ProgressEvent.ArchivePostUncookFixup, ++numArchivesProcessed, uncookedLinker.Archives.Count);
            }

            progressHandler?.Invoke(ProgressEvent.UncookComplete, uncookedLinker.Archives.Count, uncookedLinker.Archives.Count);

            return uncookedLinker;
        }

        private DirectedAcyclicGraph<FArchive> BuildDependencyTree(IEnumerable<FArchive> archives)
        {
            var dependencyTree = new DirectedAcyclicGraph<FArchive>();

            foreach (var archive in archives)
            {
                // AddEdge below will add both the source and target archive to the tree if needed. However,
                // if an archive doesn't have any dependencies (or they couldn't be loaded), that AddEdge call will
                // never happen. To make sure every archive ends up in the tree, we explicitly add them here first.
                dependencyTree.AddNode(archive);

                // TODO don't repeat this work
                foreach (var importEntry in archive.ImportTable)
                {
                    if (importEntry.IsPackage && importEntry.OuterIndex == 0 && importEntry.ObjectName != archive.NormalizedName)
                    {
                        if (InputLinker.TryGetArchiveWithNormalizedName(importEntry.ObjectName, out FArchive? depArchive))
                        {
                            dependencyTree.AddEdge(archive, depArchive!);
                        }

                    }
                    else if (archive.IsMap && importEntry.IsClass && importEntry.OuterIndex > 0)
                    {
                        // Maps have a very weird behavior where they can have a cooked copy of a class in their exports, but at the same
                        // time, the class is still listed as an import. For our purposes, we need to load the original archive if we can
                        if (importEntry.OuterTable.IsPackage && InputLinker.TryGetArchiveWithNormalizedName(importEntry.OuterTable.ObjectName, out FArchive? depArchive))
                        {
                            dependencyTree.AddEdge(archive, depArchive!);
                        }
                    }
                }
            }

            return dependencyTree;
        }

        /// <summary>
        /// Makes a decompressed copy of the input archive, closing the original. If the original archive is not compressed,
        /// just returns it instead. Note that the input archive's header data must have been read before this is called, or
        /// it will be assumed to not be compressed.
        /// </summary>
        /// <param name="inputArchive"></param>
        /// <returns></returns>
        private FArchive DecompressArchiveIfNeeded(FArchive inputArchive)
        {
            if (!inputArchive.IsBodyCompressed && !inputArchive.IsFullyCompressed)
            {
                return inputArchive;
            }

            // TODO: instead of .upk, this should use the original extension
            string archivePath = Path.Combine(tempDecompressionDirectory!.FullName, inputArchive.FileName + ".upk");

            using (UnrealDataWriter tempFileStream = new UnrealDataWriter(File.Open(archivePath, FileMode.Create)))
            {
                inputArchive.DecompressToStream(tempFileStream);
                inputArchive.EndSerialization();
            }

            // Create a new archive, replacing the previous temp file stream with a read-only one,
            // because having a write stream open will interfere with cleaning up the temp directory.
            var decompressedArchive = OpenArchiveForRead(archivePath, inputArchive.FileName);

            // Since the input archive had its header data read already, this one should too
            decompressedArchive.SerializeHeaderData();

            return decompressedArchive;
        }

        private FArchive LoadSingleArchiveUpToHeaders(string archivePath)
        {
            if (!File.Exists(archivePath))
            {
                throw new FileNotFoundException($"Input file {archivePath} could not be found");
            }

            string archiveName = Path.GetFileNameWithoutExtension(archivePath);

            if (InputLinker.TryGetArchiveWithFileName(archiveName, out FArchive? existingArchive))
            {
                _logger.LogWarning("The archive {archiveName} has already been loaded and will be skipped", archiveName);
                return existingArchive!;
            }

            var archive = OpenArchiveForRead(archivePath, archiveName);

            try
            {
                archive.SerializeHeaderData();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Could not initialize archive {fileName} - it will be skipped.", archive.FileName);
            }

            return DecompressArchiveIfNeeded(archive);
        }

        private FArchive OpenArchiveForRead(string archivePath, string archiveName)
        {
            var fileStream = File.Open(archivePath, FileMode.Open, FileAccess.Read);
            var archive = new FArchive(archiveName, InputLinker, _logger);
            archive.BeginSerialization(new UnrealDataReader(fileStream));

            return archive;
        }

        public void WriteArchiveToDisk(FArchive archive, string containingFolderPath)
        {
            string extension = archive.IsMap ? ".udk" : ".upk";
            string archivePath = Path.Combine(containingFolderPath, archive.FileName + extension);

            // Create any missing parts of the folder path, if needed
            Directory.CreateDirectory(containingFolderPath);

            var outStream = new UnrealDataWriter(File.Open(archivePath, FileMode.Create));

            archive.BeginSerialization(outStream);
            archive.SerializeHeaderData();
            archive.SerializeBodyData();

            // The first time we serialize the header, we don't know all of the sizes/offsets that we need;
            // so once the body is serialized, we go back and do the header again.
            outStream.Seek(0, SeekOrigin.Begin);
            archive.SerializeHeaderData();

            archive.EndSerialization(); // closes the stream
        }
    }
}
