﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.Intrinsic.Core;
using UnrealArchiveLibrary.Utils;
using UnrealPackageLibrary;

namespace UnrealArchiveLibrary.Unreal
{
    [Flags]
    public enum CompressionFlag
    {
        None = 0x00,
        ZLIB = 0x01,
        LZO = 0x02,
        LZX = 0x04,

        /// <summary>
        /// Indicates that when compressing, we should trade off compression speed in favor of a smaller result.
        /// </summary>
        PrioritizeSize = 0x10,

        /// <summary>
        /// Indicates that when compressing, we should trade off file size in favor of compressing faster.
        /// </summary>
        PrioritizeSpeed = 0x20,
    }

    [Flags]
    public enum PackageFlag
    {
        None = 0x00000000,
        AllowDownload = 0x00000001,
        ClientOptional = 0x00000002,
        ServerSideOnly = 0x00000004,
        Cooked = 0x00000008,
        Unsecure = 0x00000010,
        SavedWithNewerVersion = 0x00000020,
        Need = 0x00008000,
        Compiling = 0x00010000,
        ContainsMap = 0x00020000,
        Trash = 0x00040000,
        DisallowLazyLoading = 0x00080000,
        PlayInEditor = 0x00100000,
        ContainsScript = 0x00200000,
        ContainsDebugInfo = 0x00400000,
        RequireImportsAlreadyLoaded = 0x00800000,
        SelfContainedLighting = 0x01000000,
        StoreCompressed = 0x02000000,
        StoreFullyCompressed = 0x04000000,
        ContainsInlinedShaders = 0x08000000,
        ContainsFaceFXData = 0x10000000,
        NoExportAllowed = 0x20000000,
        StrippedSource = 0x40000000,
    }

    public struct FCompressedChunk : IUnrealSerializable
    {
        #region Serialized data

        public int UncompressedOffset;

        public int UncompressedSize;

        public int CompressedOffset;

        public int CompressedSize;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FCompressedChunk) sourceObj;

            UncompressedOffset = other.UncompressedOffset;
            UncompressedSize = other.UncompressedSize;
            CompressedOffset = other.CompressedOffset;
            CompressedSize = other.CompressedSize;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref UncompressedOffset);
            stream.Int32(ref UncompressedSize);
            stream.Int32(ref CompressedOffset);
            stream.Int32(ref CompressedSize);
        }
    }

    /// <summary>
    /// An FArchive represents a physical file on disk. It is associated with a top-level Unreal package,
    /// as described in the PackageFileSummary, but can also contain other packages as exports or imports.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class FArchive(string fileName, Linker linker, ILogger logger)
    {
        public const uint UNREAL_SIGNATURE = 0x9E2A83C1;
        
        // Internal so that UObjects can do logging without having to pass it everywhere
        internal readonly ILogger Log = logger;

        // Not readonly so it can be used with an UnrealDataWriter
        private static ulong NameTableFlag = (ulong) (ObjectFlag.TagExp | ObjectFlag.LoadForClient | ObjectFlag.LoadForEdit | ObjectFlag.LoadForServer);

        #region Serialized data

        // -----------------------------------------------------
        // Fields found in the package header
        // -----------------------------------------------------

        public FPackageFileSummary PackageFileSummary = new FPackageFileSummary();
        public List<string> NameTable { get; private set; } = [];
        public List<FImportTableEntry> ImportTable { get; private set; } = [];
        public List<FExportTableEntry> ExportTable { get; private set; } = [];
        public FThumbnailMetadata[] ThumbnailMetadataTable { get; private set; } = [];

        // -----------------------------------------------------
        // Data from the package body
        // -----------------------------------------------------

        /// <summary>
        /// A map of each export object to the indices of the import objects which it needs. The first-dimension
        /// length of this array is equal to the size of <see cref="ExportTable"/>. The second-dimension length
        /// for each entry depends on the export object, and may even be 0 if the exported object does not have
        /// any imported dependencies.
        /// </summary>
        /// <remarks>
        /// Note that if <see cref="HasDependsMap"/> is false, then this entire object will be null. If 
        /// it is true, then the outer array will be non-null, but the inner arrays are not created until the
        /// archive's body has been deserialized.
        /// </remarks>
        public List<int[]> DependsMap = [];

        /// <summary>
        /// The deserialized objects which this archive exports. Before <see cref="SerializeExportObjects"/> is called,
        /// most or all of the entries in this array will be null, save for any which were loaded on demand.
        /// </summary>
        public List<UObject> ExportedObjects { get; set; } = [];

        #endregion

        /// <summary>
        /// The name of this archive's file on disk. Does not include the extension.
        /// </summary>
        public string FileName { get;  set; } = fileName;

        public ArchiveFormat Format { get; private set; } = ArchiveFormat.Unknown;

        private readonly string archivePathPrefix = fileName + ".";

        /// <summary>
        /// The <see cref="Linker"/> which loaded this archive.
        /// </summary>
        public Linker ParentLinker { get; private set; } = linker;

        public string NormalizedName
        {
            get
            {
                string name = FileName;

                if (name.EndsWith("_SF"))
                {
                    name = name.Substring(0, name.Length - 3);
                }

                return name;
            }
        }

        /// <summary>
        /// Returns true if this archive has a DependsMap in it. This does not mean the map has been serialized
        /// yet, however.
        /// </summary>
        public bool HasDependsMap => PackageFileSummary.DependsOffset > 0;

        /// <summary>
        /// Whether this archive's body is stored compressed on disk. If so, the <see cref="FPackageFileSummary"/> will have
        /// details on how many compressed chunks there are and how they map to uncompresesd offsets into the archive.
        /// </summary>
        public bool IsBodyCompressed => PackageFileSummary.PackageFlags.HasFlag(PackageFlag.StoreCompressed);
        
        /// <summary>
        /// Whether this archive is fully compressed on disk; that is, the archive was written once to disk uncompressed,
        /// and then the entire archive was compressed (as opposed to only the archive's body, which would leave the header
        /// uncompressed and readable).
        /// </summary>
        public bool IsFullyCompressed => PackageFileSummary.PackageFlags.HasFlag(PackageFlag.StoreFullyCompressed);

        /// <summary>
        /// Whether this is a map package. Map packages are seekfree.
        /// </summary>
        public bool IsMap => PackageFileSummary.PackageFlags.HasFlag(PackageFlag.ContainsMap);

        /// <summary>
        /// All of the top level packages which are included in this archive.
        /// </summary>
        public List<UPackage> TopLevelPackages { get; private set; } = [];

        public bool IsCooked => PackageFileSummary.PackageFlags.HasFlag(PackageFlag.Cooked);
        public bool IsOpen => _stream != null;
        public bool IsLoading => _stream!.IsRead;
        public bool IsSaving => _stream!.IsWrite;

        private MultiValueDictionary<string, FExportTableEntry> ExportTableByObjectPath = new MultiValueDictionary<string, FExportTableEntry>();
        private IUnrealDataStream? _stream;

        private string DebuggerDisplay
        {
            get
            {
                return NormalizedName;
            }
        }

        public void BeginSerialization(IUnrealDataStream stream, bool forcePreserveData = false)
        {
            _stream = stream;
            _stream.Archive = this;

            // When we're about to save a brand new archive, we need to set some values that normally the editor would set
            if (IsSaving && !forcePreserveData)
            {
                var generation = new FGenerationInfo();

                PackageFileSummary = new FPackageFileSummary
                {
                    Signature = UNREAL_SIGNATURE,
                    FileVersion = 845,
                    LicenseeVersion = 0, // XCOM's is 64, but if we set that, our Unreal Editor won't open the package
                    EngineVersion = 8916,
                    HeaderSize = 0, // can't be calculated yet
                    FolderName = "None", // TODO
                    PackageFlags = PackageFlag.AllowDownload | PackageFileSummary.PackageFlags, // carry over any flags set during uncooking
                    NameCount = NameTable.Count,
                    NameOffset = -1,
                    ExportCount = ExportTable.Count,
                    ExportOffset = -1,
                    ImportCount = ImportTable.Count,
                    ImportOffset = -1,
                    DependsOffset = -1,
                    Generations = [ generation ],
                    AdditionalPackagesToCook = [],
                    TextureAllocations = new FTextureAllocations()
                };
            }
        }

        public void EndSerialization()
        {
            _stream?.Dispose();
            _stream = null;
        }

        public void SerializeHeaderData()
        {
            if (IsFullyCompressed)
            {
                throw new Exception($"{nameof(SerializeHeaderData)} should not be called on a fully compressed archive; decompress it first");
            }

            // Note: when writing an archive, this function will get called twice. We don't know all of the sizes/offsets
            // on the first pass, but we know that the header size won't change. Accordingly, we write the header once with
            // bad values, serialize all objects (while updating their metadata), then overwrite the header later.

            _stream!.Object(ref PackageFileSummary);

            if (Format == ArchiveFormat.Unknown)
            {
                if (PackageFileSummary.EngineVersion == 8917)
                {
                    if (PackageFileSummary.FileVersion == 845)
                    {
                        if (PackageFileSummary.LicenseeVersion == 0 || PackageFileSummary.LicenseeVersion == 64)
                        {
                            // Nearly all of the shipped EW files are version 64, except one sound bank file which seemingly got overlooked and is 0
                            Format = ArchiveFormat.XComEW;
                        }
                        else if (PackageFileSummary.LicenseeVersion > 64)
                        {
                            // There are multiple licensee versions in the WotC SDK (117, 120, maybe more)
                            Format = ArchiveFormat.XCom2WotC;
                        }
                    }
                }
            }

            // Although we could technically do on-the-fly decompression, it sounds like a real pain, so we just stop archive
            // deserialization if it's compressed, then later it'll get decompressed into a separate file and start over from there.
            if (IsBodyCompressed)
            {
                return;
            }

            SerializeNameTable();
            SerializeImportTable();
            SerializeExportTable();
            SerializeThumbnailMetadataTable();

            if (IsLoading)
            {
                ExportedObjects = new List<UObject>(new UObject[ExportTable.Count]);
            }
            else
            {
                PackageFileSummary.HeaderSize = (int) _stream.Position;
            }
        }

        /// <summary>
        /// Causes this archive to read and store all of the data outside of its header block.
        /// </summary>
        public void SerializeBodyData(IEnumerable<string>? classWhitelist = null)
        {
            SerializeDependsMap();
            SerializeExportObjects(classWhitelist);

            if (IsLoading)
            {
                ConnectInnerObjects();
                TopLevelPackages = GetTopLevelPackages();
            }
        }

        /// <summary>
        /// Causes this archive to serialize all of its class exports, so that they're available for use when
        /// reading other objects. Only call this function while loading, not when saving.
        /// </summary>
        public void SerializeClassExports()
        {
            if (IsSaving)
            {
                throw new Exception($"{nameof(FArchive)}: {nameof(SerializeClassExports)} is only valid to call when reading data, not writing");
            }

            for (int i = 0; i < ExportTable.Count; i++)
            {
                if (ExportTable[i].IsClass)
                {
                    LoadExport(i);
                }
            }
        }

        /// <summary>
        /// Reads this archive's contents, decompresses them, and writes them the
        /// uncompressed version to a different stream.
        /// </summary>
        /// <param name="outStream"></param>
        public void DecompressToStream(IUnrealDataStream outStream)
        {
            if (IsFullyCompressed)
            {
                // A fully compressed archive means everything is one giant compressed block,
                // from the first byte of the archive
                _stream!.Seek(0, SeekOrigin.Begin);
                byte[] uncompressedData = _stream.CompressedData(ECompressionMethod.LZO);
                outStream.Bytes(ref uncompressedData, uncompressedData.Length);
            }
            else if (IsBodyCompressed)
            {
                // Copy the file summary and strip any compression flags/indicators from it
                var newFileSummary = new FPackageFileSummary();
                newFileSummary.CloneFromOtherArchive(PackageFileSummary, this, outStream.Archive!);
                newFileSummary.CompressedChunks = [];
                newFileSummary.CompressionFlags = CompressionFlag.None;
                newFileSummary.PackageFlags &= ~(PackageFlag.StoreCompressed | PackageFlag.StoreFullyCompressed);
                newFileSummary.IsCopyingForDecompression = true;

                outStream.Object(ref newFileSummary);

                for (int i = 0; i < PackageFileSummary.CompressedChunks.Length; i++)
                {
                    _stream!.Seek(PackageFileSummary.CompressedChunks[i].CompressedOffset, SeekOrigin.Begin);
                    byte[] uncompressedData = _stream.CompressedData(ECompressionMethod.LZO);
                    outStream.Bytes(ref uncompressedData, uncompressedData.Length);
                }
            }
        }

        #region Functions for adding to an output archive

        /// <summary>
        /// Adds an object as an export in this archive, remapping its names and object indices as needed
        /// to make them valid within this archive.
        /// </summary>
        /// <param name="sourceObj">The object being added in this archive; must be an export from another archive. It will not be modified.</param>
        public UObject? AddExportObject(UObject sourceObj)
        {
            FExportTableEntry sourceExportTable = sourceObj.ExportTableEntry!;
            string fullObjectPath = sourceExportTable.FullObjectPath;

            // Packages don't really exist in individual uncooked archives; they're more of a divider for
            // demarking when content is combined into a single UPK via cooking
            if (sourceObj is UPackage && sourceObj.ObjectName == FileName)
            {
                return null;
            }

            if (sourceObj is UClass)
            {
                PackageFileSummary.PackageFlags |= PackageFlag.ContainsScript;
            }

            if (sourceObj.ObjectName == "TheWorld")
            {
                PackageFileSummary.PackageFlags |= PackageFlag.ContainsMap;
            }

            // TODO similar flags for other types

            // We might already have an export table entry, if another object referenced this export before we loaded it. If not,
            // then we'll want to make a new one.
            FExportTableEntry? destTableEntry = GetExportTableEntry(fullObjectPath, sourceObj.TableEntry);

            if (destTableEntry == null)
            {
                bool outerIsThisPackage = (sourceObj.Outer?.ObjectName ?? "") == FileName;

                destTableEntry = new FExportTableEntry(this)
                {
                    ClassIndex = MapIndexFromSourceArchive(sourceExportTable.ClassIndex, sourceExportTable.Archive),
                    SuperIndex = MapIndexFromSourceArchive(sourceExportTable.SuperIndex, sourceExportTable.Archive),
                    OuterIndex = outerIsThisPackage ? 0 : MapIndexFromSourceArchive(sourceExportTable.OuterIndex, sourceExportTable.Archive),
                    ObjectName = MapNameFromSourceArchive(sourceExportTable.ObjectName),
                    ArchetypeIndex = MapIndexFromSourceArchive(sourceExportTable.ArchetypeIndex, sourceExportTable.Archive),
                    ObjectFlags = sourceExportTable.ObjectFlags,
                    SerialSize = -1,
                    SerialOffset = -1,
                    ExportFlags = 0, // drop export flags; they shouldn't apply to uncooked objects
                    GenerationNetObjectCount = [], // TODO is this necessary? can we just drop it?
                    PackageGuid = sourceExportTable.PackageGuid,
                    PackageFlags = sourceExportTable.PackageFlags,
                    TableEntryIndex = ExportTable.Count
                };

                ExportTable.Add(destTableEntry);
                ExportTableByObjectPath.Add(fullObjectPath, destTableEntry);

                if (destTableEntry.TableEntryIndex >= ExportedObjects.Count)
                {
                    ExportedObjects.Add(null); // make room for the object later
                }
            }

            // TODO: this is a hack because my definition of FExportTableEntry.ClassName is stupid and I don't want to change it right now
            UObject destObj = sourceObj.ExportTableEntry.IsClass              ? new UClass(this, destTableEntry) : 
                                sourceObj.ExportTableEntry.IsClassDefaultObject ? new UObject(this, destTableEntry) : 
                                                                                UObject.NewObjectBasedOnClassName(sourceObj.ExportTableEntry.ClassName, this, destTableEntry);
            destObj.CloneFromOtherArchive(sourceObj);

            ExportedObjects[destTableEntry.TableEntryIndex] = destObj;
            DependsMap.Add(Array.Empty<int>());

            return destObj;
        }

        /// <summary>
        /// Adds an entry to the import table. Does NOT check to make sure an identical entry doesn't already exist, so use with caution.
        /// </summary>
        public void AddImportObject(string ClassPackage, string ClassName, int OuterIndex, string ObjectName, FArchive? sourceArchive = null)
        {
            var importObj = new FImportTableEntry(this)
            { 
                ClassPackage = GetOrCreateName(ClassPackage),
                _className = GetOrCreateName(ClassName),
                OuterIndex = sourceArchive == null ? OuterIndex : MapIndexFromSourceArchive(OuterIndex, sourceArchive),
                ObjectName = GetOrCreateName(ObjectName),
                TableEntryIndex = ImportTable.Count
            };

            ImportTable.Add(importObj);
        }

        public FName GetOrCreateName(string name)
        {
            int index = NameTable.IndexOf(name);

            if (index < 0)
            {
                index = NameTable.Count;
                NameTable.Add(name);
            }

            return new FName()
            {
                Index = index,
                Suffix = 0,
                Archive = this
            };
        }

        /// <summary>
        /// Finds an existing import matching the inputs, or creates one
        /// and returns its index if one doesn't already exist.
        /// </summary>
        /// <param name="classPackage">The package where the object's class is found.</param>
        /// <param name="className">The class of the object being imported.</param>
        /// <param name="objectName">The name of the object to import.</param>
        /// <param name="outerIndex">The outer index of the object; must be 0 (none) or negative (indicating another import).</param>
        /// <returns>The import index of the entry.</returns>
        public int GetOrCreateImport(FName classPackage, FName className, FName objectName, int outerIndex)
        {
#if DEBUG
            if (outerIndex > 0)
            {
                throw new Exception($"The outer index of an import cannot be an export; received {outerIndex}");
            }
#endif

            int importIndex = ImportTable.FindIndex(imp => imp.OuterIndex == outerIndex
                                                            && imp.ObjectName == objectName
                                                            && imp.ClassPackage == classPackage!
                                                            && imp.ClassName == className);

            if (importIndex >= 0)
            {
                return -1 * (importIndex + 1);
            }

            // Didn't find an existing import; add a new import entry instead
            FImportTableEntry importEntry = new FImportTableEntry(this)
            {
                ClassPackage = classPackage!,
                _className = className,
                OuterIndex = outerIndex,
                ObjectName = objectName,
                TableEntryIndex = ImportTable.Count
            };

            importIndex = -1 * (ImportTable.Count + 1);
            ImportTable.Add(importEntry);
            return importIndex;
        }

        /// <summary>
        /// Takes an object index from the given source archive, and remaps it to be a part of this archive.
        /// This may change it from an import to an export or vice versa, depending on where the object originally
        /// resided, and where it will be after uncooking. If this requires a new entry in this archive's import
        /// or export table, then one will be added automatically.
        /// </summary>
        /// <param name="index">An object index which originated in the source archive.</param>
        /// <param name="source">The archive where the index came from.</param>
        /// <returns>The import or export index, or 0 if something went wrong and no index could be determined. Also
        /// returns 0 if the incoming index is 0.</returns>
        public int MapIndexFromSourceArchive(int index, FArchive source)
        {
            if (index == 0)
            {
                return 0;
            }

            FObjectTableEntry sourceTableEntry = source.GetObjectTableEntry(index);
            string objectClassName = sourceTableEntry.ClassName;

            // These are intrinsic classes which can't be exported during uncooking
            if (objectClassName == "VisGroupActor" || objectClassName == "XComWorldDataContainer")
            {
                return 0;
            }

            string fullObjectPath = sourceTableEntry.FullObjectPath;
            string uncookedArchiveName = ParentLinker.GetUncookedArchiveNameForObject(fullObjectPath);
            bool isIntrinsic = IsIntrinsicObject(fullObjectPath);

            if (uncookedArchiveName == "")
            {
                // Some top-level packages won't be returned by GetUncookedArchiveNameForObject, e.g. if there wasn't
                // a UPackage object in the cooked archive. This is most likely to occur for archives that are mostly scripts.
                if (sourceTableEntry.IsPackage && sourceTableEntry.OuterIndex == 0)
                {
                    uncookedArchiveName = sourceTableEntry.ObjectName;
                }
                else if (fullObjectPath == "UnrealEd.CascadeParticleSystemComponent")
                {
                    // Somehow this class is referenced in an archive but doesn't seem to have shipped with the game?
                    uncookedArchiveName = "UnrealEd";
                }
            }

            // TODO: class imports keep getting all screwed up. Either fix whatever made this check necessary, or just
            // handle class imports specially so they don't go through all this process
            if (!isIntrinsic && uncookedArchiveName == "")
            {
                return 0;
            }
            else if (uncookedArchiveName == FileName && !isIntrinsic)
            {
                // This object is going to end up in our exports; check if it's already in here
                FExportTableEntry destExportEntry = GetExportTableEntry(fullObjectPath, sourceTableEntry);

                if (destExportEntry != null)
                {
                    return 1 + destExportEntry.TableEntryIndex;
                }

                // Don't add export table entries for this package
                if (sourceTableEntry.ObjectName == FileName && sourceTableEntry.ClassName == "Package")
                {
                    return 0;
                }

                // Get a cooked version of the object we're going to be exporting, and use that to determine
                // our export table properties
                UObject sourceExportObject = source.ParentLinker.GetCookedObjectByPath(sourceTableEntry.FullObjectPath, sourceTableEntry);

                // If only some of a game's archives are loaded, then we may be aware of export objects for a package, but not actually
                // have a copy of them to create an export from. For example, a cooked package C may import a startup object from package S.
                // If package S isn't loaded, then C will know about the object, but we have no way to uncook it.
                if (sourceExportObject == null)
                {
                    return 0;
                }

                FExportTableEntry sourceExportTable = sourceExportObject.ExportTableEntry;

                destExportEntry = new FExportTableEntry(this)
                {
                    ClassIndex     = MapIndexFromSourceArchive(sourceExportTable.ClassIndex, sourceExportTable.Archive),
                    SuperIndex     = MapIndexFromSourceArchive(sourceExportTable.SuperIndex, sourceExportTable.Archive),
                    OuterIndex     = MapIndexFromSourceArchive(sourceExportTable.OuterIndex, sourceExportTable.Archive),
                    ObjectName     = MapNameFromSourceArchive(sourceExportTable.ObjectName),
                    ArchetypeIndex = MapIndexFromSourceArchive(sourceExportTable.ArchetypeIndex, sourceExportTable.Archive),
                    ObjectFlags    = sourceExportTable.ObjectFlags,
                    SerialSize     = -1,
                    SerialOffset   = -1,
                    ExportFlags    = 0, // drop export flags; they shouldn't apply to uncooked objects
                    GenerationNetObjectCount = [], // TODO is this necessary? can we just drop it?
                    PackageGuid    = sourceExportTable.PackageGuid,
                    PackageFlags   = sourceExportTable.PackageFlags,
                    TableEntryIndex = ExportTable.Count
                };

                int exportIndex = ExportTable.Count + 1;
                ExportTable.Add(destExportEntry);
                ExportTableByObjectPath.Add(fullObjectPath, destExportEntry);

                if (exportIndex >= ExportedObjects.Count)
                {
                    ExportedObjects.Add(null); // leave room for the object later
                }

                return exportIndex;
            }
            else
            {
                // Before we add an import, we need to make sure this isn't a private object which got cooked
                // into a package (probably a map). That would cause the uncooked package to be unopenable.
                // Theoretically, if this is a problem, the source table entry will be from a cooked package
                // and therefore we'll have an FExportTableEntry to read from. That saves us an expensive lookup
                // for the object by name.
                var sourceExportEntry = sourceTableEntry as FExportTableEntry;
                if (sourceExportEntry != null)
                {
                    if (!sourceExportEntry.ObjectFlags.HasFlag(ObjectFlag.Public))
                    {
                        return 0;
                    }
                }

                // This will be an import; see if we already have the same import
                FName objectName = MapNameFromSourceArchive(sourceTableEntry.ObjectName);
                FName className = sourceTableEntry.IsClass ? GetOrCreateName("Class") : MapNameFromSourceArchive(sourceTableEntry.ClassName);
                FName classPackage = null;

                if (sourceTableEntry.IsClass)
                {
                    // If this is a class, it'll always be a "Core.Class"
                    classPackage = GetOrCreateName("Core");
                }
                else if (sourceTableEntry is FImportTableEntry sourceImportEntry)
                {
                    classPackage = MapNameFromSourceArchive(sourceImportEntry.ClassPackage);
                }
                else if (sourceExportEntry != null)
                {
                    // If this is an export, then the class package has to be pulled more indirectly
                    var sourceClassEntry = sourceTableEntry.Archive.GetObjectTableEntry(sourceExportEntry.ClassIndex);

                    // Not every class will have an outer, but use it if it's there
                    if (sourceClassEntry != null && sourceClassEntry.OuterIndex != 0)
                    {
                        classPackage = MapNameFromSourceArchive(sourceClassEntry.OuterTable.ObjectName);
                    }

                    // Otherwise the outer should just be the uncooked name
                    if (classPackage is null)
                    {
                        classPackage = GetOrCreateName(uncookedArchiveName);
                    }
                }
                else
                {
                    // can't ever get here; there's only two subclasses of FObjectTableEntry
                }

                int destOuterIndex = 0;

                if (sourceTableEntry.OuterIndex != 0)
                {
                    destOuterIndex = MapIndexFromSourceArchive(sourceTableEntry.OuterIndex, sourceTableEntry.Archive);
                }
                else if (uncookedArchiveName != "" && !sourceTableEntry.IsPackage)
                {
                    // If the object has no outer index, then it's already in its uncooked destination. If it's a package,
                    // skip adding an outer import; top level packages don't have one.
                    destOuterIndex = GetOrCreateImport(GetOrCreateName("Core"), GetOrCreateName("Package"), GetOrCreateName(uncookedArchiveName), /* outerIndex */ 0);
                }

                return GetOrCreateImport(classPackage!, className, objectName, destOuterIndex);
            }
        }

        public int[] MapIndicesFromSourceArchive(int[] indices, FArchive source)
        {
            int[] output = new int[indices.Length];

            for (int i = 0; i < indices.Length; i++)
            {
                output[i] = MapIndexFromSourceArchive(indices[i], source);
            }

            return output;
        }

        public IList<int> MapIndicesFromSourceArchive(IList<int> indices, FArchive source)
        {
            var output = new List<int>(indices.Count);

            for (int i = 0; i < indices.Count; i++)
            {
                var index = MapIndexFromSourceArchive(indices[i], source);
                output.Add(index);
            }

            return output;
        }

        public FName? MapNameFromSourceArchive(FName? name)
        {
            if (name is null)
            {
                return null;
            }

            string nameAsString = name.WithoutSuffix();

            // Check if this same entry is already in our name table
            int destNameTableIndex = NameTable.IndexOf(nameAsString);

            // If not, make a new entry
            if (destNameTableIndex < 0)
            {
                destNameTableIndex = NameTable.Count;
                NameTable.Add(nameAsString);
            }

            return new FName()
            { 
                Index = destNameTableIndex,
                Suffix = name.Suffix,
                Archive = this
            };
        }

        #endregion

        public FName GetClassName(int index)
        {
            return index < 0 ? GetImportTableEntry(index).ObjectName : GetExportTableEntry(index).ObjectName;
        }

        /// <summary>
        /// Retrieves an exported object by its path, if it is in this archive. For archives which are being
        /// deserialized from disk, this shouldn't be called until after <see cref="SerializeBodyData"/> is called.
        /// </summary>
        public UObject? GetExportedObjectByPath(string path, FObjectTableEntry tableEntry)
        {
            // Look for top level objects matching the first part of the path
            if (ExportTableByObjectPath.TryGetValue(path, out var exportTableEntries))
            {
                for (int i = 0; i < exportTableEntries.Count; i++)
                {
                    if (exportTableEntries[i].ClassName == tableEntry.ClassName)
                    {
                        return exportTableEntries[i].ExportObject ?? LoadExport(exportTableEntries[i].TableEntryIndex);
                    }
                }
            }

            // Some objects are implicitly exported under the archive's name and don't have a top level UPackage to contain
            // them, so try again with the first part of the path stripped if that might be the case here
            if (path.StartsWith(archivePathPrefix))
            {
                path = path.Substring(archivePathPrefix.Length);
                return GetExportedObjectByPath(path, tableEntry);
            }

            return null;
        }

        public FExportTableEntry GetExportTableEntry(int index)
        {
            // Export indices are always positive
            int translatedIndex = index - 1;

            return ExportTable[translatedIndex];
        }

        /// <summary>
        /// Gets the export table entry for an object, without requiring the exported object to be present.
        /// Useful when creating an archive in-memory; sometimes we need to reference an export before the
        /// actual object is available.
        /// </summary>
        /// <param name="fullObjectPath"></param>
        /// <returns>The table entry if found, or null otherwise.</returns>
        public FExportTableEntry? GetExportTableEntry(string fullObjectPath, FObjectTableEntry tableEntry)
        {
            if (ExportTableByObjectPath.TryGetValue(fullObjectPath, out var exportEntries))
            {
                for (int i = 0; i < exportEntries.Count; i++)
                {
                    if (exportEntries[i].ClassName == tableEntry.ClassName)
                    {
                        return exportEntries[i];
                    }
                }
            }

            return null;
        }

        public FImportTableEntry GetImportTableEntry(int index)
        {
            // Import indices are always negative
            int translatedIndex = -1 * (index + 1);

            return ImportTable[translatedIndex];
        }

        public FObjectTableEntry GetObjectTableEntry(int index)
        {
            if (index == 0)
            {
                return null;
            }

            return index < 0 ? GetImportTableEntry(index) : GetExportTableEntry(index);
        }

        public string GetNameStringByIndex(int index)
        {
            if (index < 0 || index >= NameTable.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "not in name table");
            }

            return NameTable[index];
        }

        public string NameToString(FName name)
        {
            string value = GetNameStringByIndex(name.Index);

            if (name.Suffix == 0)
            {
                return value;
            }

            return $"{value}_{name.Suffix - 1}";
        }

        /// <summary>
        /// Gets an object by its index. Export objects will be retrieved from this archive; imported objects
        /// will be retrieved from another loaded archive, if any contain the requested path.
        /// </summary>
        /// <param name="index">The object index to retrieve. Can be an import or an export index.</param>
        /// <returns>The requested object. If not found, or if index is 0, returns null.</returns>
        public UObject? GetObjectByIndex(int index)
        {
            if (index == 0)
            {
                return null;
            }

            if (index < 0)
            {
                int importTableIndex = -1 * (index + 1);
                return ParentLinker.GetCookedObjectByPath(ImportTable[importTableIndex].FullObjectPath, ImportTable[importTableIndex]);
            }

            int exportTableIndex = index - 1;

            if (exportTableIndex >= ExportedObjects.Count)
            {
                throw new ArgumentException($"{nameof(GetObjectByIndex)}: requested index {index} ref of bounds. Only {ExportedObjects.Count} export objects exist in archive {FileName}.");
            }

            if (ExportedObjects[exportTableIndex] == null && IsLoading)
            {
                long previousPosition = _stream.Position;
                ExportedObjects[exportTableIndex] = LoadExport(exportTableIndex);
                _stream.Seek(previousPosition, SeekOrigin.Begin);
            }

            return ExportedObjects[exportTableIndex];
        }

        /// <summary>
        /// Links all of the export objects in this archive to the objects for which they are the Outer object.
        /// </summary>
        private void ConnectInnerObjects()
        {
            for (int i = 0; i < ExportedObjects.Count; i++)
            {
                UObject outer = ExportedObjects[i]?.Outer;

                if (outer != null)
                {
                    outer.InnerObjects.Add(ExportedObjects[i]);
                }
            }
        }

        /// <summary>
        /// Finds all of the top-level packages in this archive (i.e. exported packages which have no Outer object).
        /// </summary>
        private List<UPackage> GetTopLevelPackages()
        {
            var packages = new List<UPackage>();

            for (int i = 0; i < ExportedObjects.Count; i++)
            {
                if (ExportedObjects[i] == null)
                {
                    continue;
                }

                if (ExportedObjects[i].TableEntry.OuterIndex != 0)
                {
                    continue;
                }

                if (ExportedObjects[i] is UPackage pkg)
                {
                    packages.Add(pkg);
                }
            }

            return packages;
        }

        /// <summary>
        /// Checks if the given object is intrinsic, meaning it exists in the Unreal engine, but doesn't have a corresponding object
        /// export anywhere in the archive files. This is seemingly something UE3 does for bootstrapping, so that it can have a small 
        /// set of types which always exist internally.
        /// </summary>
        /// <param name="fullObjectPath">The full path of the object to check.</param>
        /// <returns>True if this object only exists as an import; false if it must have a corresponding export somewhere.</returns>
        private bool IsIntrinsicObject(string fullObjectPath)
        {
            switch (fullObjectPath)
            {
                // UPackages
                case "Core":
                case "Engine":

                // Various pieces of the type system
                case "Core.ArrayProperty":
                case "Core.BoolProperty":
                case "Core.ByteProperty":
                case "Core.Class":
                case "Core.ClassProperty":
                case "Core.ComponentProperty":
                case "Core.Const":
                case "Core.DelegateProperty":
                case "Core.Enum":
                case "Core.Field":
                case "Core.FloatProperty":
                case "Core.Function":
                case "Core.InterfaceProperty":
                case "Core.IntProperty":
                case "Core.MapProperty":
                case "Core.NameProperty":
                case "Core.ObjectProperty":
                case "Core.RotatorProperty":
                case "Core.ScriptStruct":
                case "Core.State":
                case "Core.StrProperty":
                case "Core.Struct":
                case "Core.StructProperty":
                case "Core.VectorProperty":

                // Miscellaneous
                case "Core.MetaData":
                case "Core.Package":
                case "Core.ObjectRedirector": // these should probably never show up in cooked packages
                case "Core.System":
                case "Core.TextBuffer":
                case "Core.TextBufferFactory":
                case "Engine.ActorChannel":
                case "Engine.ChannelDownload":
                case "Engine.ChildConnection":
                case "Engine.Client":
                case "Engine.CodecMovieBink":
                case "Engine.ControlChannel":
                case "Engine.FileChannel":
                case "Engine.FracturedStaticMesh":
                case "Engine.GuidCache":
                case "Engine.Level":
                case "Engine.LevelBase":
                case "Engine.LightMapTexture2D":
                case "Engine.Model":
                case "Engine.PendingLevel":
                case "Engine.NavigationMeshBase":
                case "Engine.NetConnection":
                case "Engine.NetPendingLevel":
                case "Engine.PackageMapLevel":
                case "Engine.PackageMapSeekFree":
                case "Engine.Polys":
                case "Engine.Selection":
                case "Engine.ShaderCache":
                case "Engine.ShadowMap1D":
                case "Engine.StaticMesh":
                case "Engine.VoiceChannel":
                case "Engine.World":
                case "UnrealEd.OptionsProxy":
                case "UnrealEd.TexAligner":

                // Added in XCOM
                case "UnrealEd.VisGroupActor":
                case "XComGame.XComWorldDataContainer":
                    return true;

            }

            return false;
        }

        /// <summary>
        /// Loads an export object and stores it. 
        /// </summary>
        /// <param name="index">The 0-based index of the export object.</param>
        private UObject LoadExport(int i)
        {
            string exportClassName;

            if (ExportTable[i].ClassIndex == 0)
            {
                exportClassName = "Class";
            }
            else if (ExportTable[i].ClassIndex > 0)
            {
                var classObjName = GetExportTableEntry(ExportTable[i].ClassIndex).ObjectName;
                exportClassName = NameToString(classObjName);
            }
            else
            {
                var classObjName = GetImportTableEntry(ExportTable[i].ClassIndex).ObjectName;
                exportClassName = NameToString(classObjName);
            }

            // ClassDefaultObjects have the same class as an actual object of their type, but they don't have any of the custom serialized data, so they should
            // just be treated as ordinary UObjects from our perspective
            var exportObj = ExportTable[i].IsClassDefaultObject ? new UObject(this, ExportTable[i]) : UObject.NewObjectBasedOnClassName(exportClassName, this, ExportTable[i]);

            _stream.Seek(ExportTable[i].SerialOffset, SeekOrigin.Begin);
            exportObj.Serialize(_stream);

#if DEBUG
            int expectedEndPosition = ExportTable[i].SerialOffset + ExportTable[i].SerialSize;

            if (_stream.Position != expectedEndPosition)
            {
                long extraBytes = expectedEndPosition - _stream.Position;
                Log.LogWarning("In archive {fileName}, object {fullObjectPath} did not fully deserialize its data ({extraBytes} bytes remaining). Class is {className}", FileName, exportObj.FullObjectPath, extraBytes, ExportTable[i].ClassName);
            }
#endif
            return exportObj;
        }

        /// <summary>
        /// Reads the map of export objects to their imported dependencies.
        /// </summary>
        private void SerializeDependsMap()
        {
            if (IsLoading)
            {
                if (!HasDependsMap)
                {
                    return;
                }
            
                DependsMap = new List<int[]>(ExportTable.Count);
                _stream.Seek(PackageFileSummary.DependsOffset, SeekOrigin.Begin);

                // The DependsMap length is implicitly the number of exported objects
                for (int i = 0; i < ExportTable.Count; i++)
                {
                    int[] dependsArray = [];
                    _stream.Int32Array(ref dependsArray);
                    DependsMap.Add(dependsArray);
                }
            }
            else
            {
                PackageFileSummary.DependsOffset = (int) _stream.Position;

                for (int i = 0; i < ExportTable.Count; i++)
                {
                    var dependsList = new List<int>();
                    ExportedObjects[i].PopulateDependencies(dependsList);

                    // Filter out 0 entries here so every single object doesn't have to check its indices
                    var dependsArray = dependsList.Where(index => index != 0).Distinct().ToArray();
                    _stream.Int32Array(ref dependsArray);
                }
            }
        }

        /// <summary>
        /// Deserializes all of the export objects which are contained in this archive.
        /// </summary>
        private void SerializeExportObjects(IEnumerable<string>? classWhitelist = null)
        {
            int numAlreadyLoaded = 0, numFailed = 0, numSucceeded = 0;

            for (int i = 0; i < ExportTable.Count; i++)
            {
                // This export may have been preloaded by another one - skip it if so
                if (IsLoading && ExportedObjects[i] != null)
                {
                    numAlreadyLoaded++;
                    continue;
                }

                try
                {
                    if (IsLoading)
                    {
                        if (classWhitelist != null)
                        {
                            string className = ExportTable[i].ClassNameString;

                            if (!classWhitelist.Contains(className))
                            {
                                continue;
                            }
                        }

                        ExportedObjects[i] = LoadExport(i);
                    }
                    else
                    {
                        int startPosition = (int) _stream.Position;
                        ExportedObjects[i].ExportTableEntry.SerialOffset = startPosition;

                        ExportedObjects[i].Serialize(_stream);

                        int endPosition = (int) _stream.Position;
                        ExportedObjects[i].ExportTableEntry.SerialSize = endPosition - startPosition;
                    }

                    numSucceeded++;
                }
                catch
                {
                    numFailed++;
                }
            }

            if (numFailed > 0)
            {
                Log.LogWarning("Archive {fileName}: done serializing export objects. {numSucceeded} succeeded and {numFailed} failed deserialization. {numAlreadyLoaded} were previously loaded.", FileName, numSucceeded, numFailed, numAlreadyLoaded);
            }
        }

        private void SerializeExportTable()
        {
            if (IsLoading)
            {
                ExportTable = new List<FExportTableEntry>(new FExportTableEntry[PackageFileSummary.ExportCount]);
                _stream.Seek(PackageFileSummary.ExportOffset, SeekOrigin.Begin);
            }
            else
            {
                PackageFileSummary.ExportOffset = (int) _stream.Position;
            }

            if (PackageFileSummary.ExportCount == 0)
            {
                return;
            }

            for (int i = 0; i < PackageFileSummary.ExportCount; i++)
            {
                if (IsLoading)
                {
                    ExportTable[i] = new FExportTableEntry(this);
                    ExportTable[i].TableEntryIndex = i;
                }

                ExportTable[i].Serialize(_stream);
            }

            if (IsLoading)
            {
                for (int i = 0; i < PackageFileSummary.ExportCount; i++)
                {
                    ExportTableByObjectPath.Add(ExportTable[i].FullObjectPath, ExportTable[i]);
                }
            }
        }

        private void SerializeImportTable()
        {
            if (IsLoading)
            {
                ImportTable = new List<FImportTableEntry>(new FImportTableEntry[PackageFileSummary.ImportCount]);
                _stream.Seek(PackageFileSummary.ImportOffset, SeekOrigin.Begin);
            }
            else
            {
                PackageFileSummary.ImportOffset = (int) _stream.Position;
            }

            if (PackageFileSummary.ImportCount == 0)
            {
                return;
            }

            for (int i = 0; i < PackageFileSummary.ImportCount; i++)
            {
                if (IsLoading)
                {
                    ImportTable[i] = new FImportTableEntry(this);
                    ImportTable[i].TableEntryIndex = i;
                }

                ImportTable[i].Serialize(_stream);
            }
        }

        private void SerializeNameTable()
        {
            if (IsLoading)
            {
                NameTable = new List<string>(new string[PackageFileSummary.NameCount]);
                _stream.Seek(PackageFileSummary.NameOffset, SeekOrigin.Begin);
            }
            else
            {
                PackageFileSummary.NameOffset = (int) _stream.Position;
            }

            if (PackageFileSummary.NameCount == 0)
            {
                return;
            }

            for (int i = 0; i < PackageFileSummary.NameCount; i++)
            {
                // Weird use of a local variable to make this work for both reading and writing
                string name = NameTable[i];
                _stream.String(ref name);
                NameTable[i] = name;

                // Every name has a 64-bit object flag field, but we don't care about them when reading,
                // so save some memory when uncooking. When saving, give every name the same flags, which
                // seems to be the case anyway.
                if (IsLoading)
                {
                    _stream.SkipBytes(8);
                }
                else
                {
                    _stream.UInt64(ref NameTableFlag);
                }
            }
        }
     
        private void SerializeThumbnailMetadataTable()
        {
            // Check if there's a thumbnail table in the first place
            if (PackageFileSummary.ThumbnailTableOffset == 0)
            {
                ThumbnailMetadataTable = [];
                return;
            }

            // The below code is just for reference; all of the XCOM packages appear to have their thumbnail
            // tables stripped, although their ThumbnailTableOffset is still set for some reason

#if false
            _stream.Seek(PackageFileSummary.ThumbnailTableOffset, SeekOrigin.Begin);

            _stream.Int32(ref int thumbnailCount);
            ThumbnailMetadataTable = new FThumbnailMetadata[thumbnailCount];

            for (int i = 0; i < thumbnailCount; i++)
            {
                _stream.ThumbnailMetadata(ref ThumbnailMetadataTable[i]);
            }
#endif
        }
    }
}
