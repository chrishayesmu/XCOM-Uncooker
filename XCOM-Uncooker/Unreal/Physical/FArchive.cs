using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core;

namespace XCOM_Uncooker.Unreal.Physical
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
    }

    /// <summary>
    /// An FArchive represents a physical file on disk. It is associated with a top-level Unreal package,
    /// as described in the PackageFileSummary, but can also contain other packages as exports or imports.
    /// Those packages are effectively just
    /// </summary>
    public class FArchive(string fileName, Linker linker)
    {
        const uint UNREAL_SIGNATURE = 0x9E2A83C1;

        private static readonly Logger Log = new Logger(nameof(FArchive));

        #region Serialized data

        // -----------------------------------------------------
        // Fields found in the package header
        // -----------------------------------------------------

        public FPackageFileSummary PackageFileSummary = new FPackageFileSummary();
        public List<string> NameTable { get; private set; } = [];
        public List<FImportTableEntry> ImportTable { get; private set; } = [];
        public List<FExportTableEntry> ExportTable { get; private set; } = [];
        public FThumbnailMetadata[] ThumbnailMetadataTable { get; private set; }

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
        public int[][] DependsMap;

        /// <summary>
        /// The deserialized objects which this archive exports. Before <see cref="SerializeExportObjects"/> is called,
        /// most or all of the entries in this array will be null, save for any which were loaded on demand.
        /// </summary>
        public List<UObject> ExportedObjects { get; private set; } = [];

        #endregion

        /// <summary>
        /// The name of this archive's file on disk.
        /// </summary>
        public string FileName { get; private set; } = fileName;

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
        /// Whether this is a map package. Map packages are seekfree.
        /// </summary>
        public bool IsMap => PackageFileSummary.PackageFlags.HasFlag(PackageFlag.ContainsMap);

        /// <summary>
        /// All of the top level packages which are included in this archive.
        /// </summary>
        public List<UPackage> TopLevelPackages { get; private set; }

        public bool IsCooked => PackageFileSummary.PackageFlags.HasFlag(PackageFlag.Cooked);
        public bool IsLoading => _stream.IsRead;
        public bool IsSaving => _stream.IsWrite;

        private IDictionary<string, FExportTableEntry> ExportTableByObjectPath = new Dictionary<string, FExportTableEntry>();
        private UnrealDataReader _stream;

        public void BeginSerialization(Stream stream)
        {
            if (stream is UnrealDataReader udStream)
            {
                _stream = udStream;
            }
            else
            {
                _stream = new UnrealDataReader(stream);
            }

            _stream.Archive = this;

            // When we're abref to save a brand new archive, we need to set some values that normally the editor would set
            if (IsSaving)
            {
                PackageFileSummary = new FPackageFileSummary
                {
                    Signature = UNREAL_SIGNATURE,
                    FileVersion = 845,
                    LicenseeVersion = 0, // XCOM's is 64, but if we set that, our Unreal Editor won't open the package
                    HeaderSize = 0, // can't be calculated yet
                    FolderName = "", // TODO
                    PackageFlags = PackageFlag.AllowDownload | PackageFlag.AllowDownload, // TODO, some conditional flags e.g. Map
                    NameCount = NameTable.Count,
                    NameOffset = -1,
                    ExportCount = ExportTable.Count,
                    ExportOffset = -1,
                    ImportCount = ImportTable.Count,
                    ImportOffset = -1,
                    DependsOffset = -1
                };
            }
        }

        public void EndSerialization()
        {
            _stream.Close();
            _stream.Dispose();
        }

        public void SerializeHeaderData()
        {
            SerializeFileSummary();
            SerializeNameTable();
            SerializeImportTable();
            SerializeExportTable();
            SerializeThumbnailMetadataTable();

            if (HasDependsMap)
            {
                DependsMap = new int[ExportTable.Count][];
            }

            // Keep ExportedObjects ready to populate in case something is loaded directly by another archive
            ExportedObjects = new List<UObject>(new UObject[ExportTable.Count]);
        }

        /// <summary>
        /// Causes this archive to read and store all of the data outside of its header block.
        /// </summary>
        public void SerializeBodyData(ProgressBar progressBar)
        {
            lock (_stream)
            {
                SerializeDependsMap();
                SerializeExportObjects(progressBar);
            }

            ConnectInnerObjects();

            if (IsLoading)
            {
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

        #region Functions for adding to an output archive

        /// <summary>
        /// Adds an object as an export in this archive, remapping its names and object indices as needed
        /// to make them valid within this archive.
        /// </summary>
        /// <param name="sourceObj">The object being added in this archive; must be an export from another archive. It will not be modified.</param>
        /// <returns>The newly created object which now exists in this archive.</returns>
        public UObject AddExportObject(UObject sourceObj)
        {
            FExportTableEntry sourceExportTable = sourceObj.ExportTableEntry;
            string fullObjectPath = sourceExportTable.FullObjectPath;
            FExportTableEntry destTableEntry = GetExportTableEntry(fullObjectPath);

            // We might already have an export table entry, if another object referenced this export before we loaded it. If not,
            // then we'll want to make a new one.
            if (destTableEntry == null)
            {
                destTableEntry = new FExportTableEntry(this)
                {
                    ClassIndex = MapIndexFromSourceArchive(sourceExportTable.ClassIndex, sourceExportTable.Archive),
                    SuperIndex = MapIndexFromSourceArchive(sourceExportTable.SuperIndex, sourceExportTable.Archive),
                    OuterIndex = MapIndexFromSourceArchive(sourceExportTable.OuterIndex, sourceExportTable.Archive),
                    ObjectName = MapNameFromSourceArchive(sourceExportTable.ObjectName),
                    ArchetypeIndex = MapIndexFromSourceArchive(sourceExportTable.ArchetypeIndex, sourceExportTable.Archive),
                    ObjectFlags = sourceExportTable.ObjectFlags,
                    SerialSize = -1,
                    SerialOffset = -1,
                    ExportFlags = sourceExportTable.ExportFlags,
                    GenerationNetObjectCount = [], // TODO is this necessary? can we just drop it?
                    PackageGuid = sourceExportTable.PackageGuid,
                    PackageFlags = sourceExportTable.PackageFlags,
                    TableEntryIndex = ExportTable.Count
                };

                ExportTable.Add(destTableEntry);
                ExportTableByObjectPath[fullObjectPath] = destTableEntry;
            }

            // TODO: this is a hack because my definition of FExportTableEntry.ClassName is stupid and I don't want to change it right now
            UObject destObj = sourceObj.ExportTableEntry.IsClass ? new UClass(this, destTableEntry) : UObject.NewObjectBasedOnClassName(sourceObj.ExportTableEntry.ClassName, this, destTableEntry);
            destObj.CloneFromOtherArchive(sourceObj);
            ExportedObjects.Add(destObj);

            return destObj;
        }

        public void AddImportObject(string ClassPackage, string ClassName, int OuterIndex, string ObjectName, FArchive sourceArchive = null)
        {
            var importObj = new FImportTableEntry(this)
            { 
                ClassPackage = GetOrCreateName(ClassPackage),
                _className = GetOrCreateName(ClassName),
                OuterIndex = sourceArchive == null ? 0 : MapIndexFromSourceArchive(OuterIndex, sourceArchive),
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
            string fullObjectPath = sourceTableEntry.FullObjectPath;
            string uncookedArchiveName = ParentLinker.GetUncookedArchiveNameForObject(fullObjectPath);

            if (uncookedArchiveName == "")
            {
                return 0;
            }
            else if (uncookedArchiveName == FileName && !IsIntrinsicObject(fullObjectPath))
            {
                // This object is going to end up in our exports; check if it's already in here
                FExportTableEntry destExportEntry = GetExportTableEntry(fullObjectPath);

                if (destExportEntry != null)
                {
                    return 1 + destExportEntry.TableEntryIndex;
                }

                // Get a cooked version of the object we're going to be exporting, and use that to determine
                // our export table properties
                UObject sourceExportObject = ParentLinker.GetCookedObjectByPath(fullObjectPath);

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
                    ExportFlags    = sourceExportTable.ExportFlags,
                    GenerationNetObjectCount = [], // TODO is this necessary? can we just drop it?
                    PackageGuid    = sourceExportTable.PackageGuid,
                    PackageFlags   = sourceExportTable.PackageFlags,
                    TableEntryIndex = ExportTable.Count
                };

                int exportIndex = ExportTable.Count + 1;
                ExportTable.Add(destExportEntry);
                ExportTableByObjectPath[fullObjectPath] = destExportEntry;
                return exportIndex;
            }
            else
            {
                // This will be an import; see if we already have the same import
                int destOuterIndex = MapIndexFromSourceArchive(sourceTableEntry.OuterIndex, sourceTableEntry.Archive);
                FName objectName = MapNameFromSourceArchive(sourceTableEntry.ObjectName), className = MapNameFromSourceArchive(sourceTableEntry.ClassName);
                FName classPackage = null;

                if (sourceTableEntry is FImportTableEntry sourceImportEntry)
                {
                    classPackage = sourceImportEntry.ClassPackage;
                }
                else if (sourceTableEntry is FExportTableEntry sourceExportEntry)
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

                int destImportIndex = ImportTable.FindIndex(imp => imp.OuterIndex == destOuterIndex 
                                                                && imp.ObjectName == objectName 
                                                                && imp.ClassPackage == classPackage! 
                                                                && imp.ClassName == className);

                if (destImportIndex >= 0)
                {
                    return -1 * (destImportIndex + 1);
                }

                // Didn't find an existing import; add a new import entry instead
                FImportTableEntry destImportEntry = new FImportTableEntry(this)
                {
                    ClassPackage = classPackage!,
                    _className = className,
                    OuterIndex = destOuterIndex,
                    ObjectName = objectName,
                    TableEntryIndex = ImportTable.Count
                };

                destImportIndex = -1 * (ImportTable.Count + 1);
                ImportTable.Add(destImportEntry);
                return destImportIndex;
            }
        }

        public FName MapNameFromSourceArchive(FName name)
        {
            if (name == null)
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
        public UObject GetExportedObjectByPath(string path)
        {
            // Look for a top level object matching the first part of the path; there should only be one at most
            ExportTableByObjectPath.TryGetValue(path, out FExportTableEntry exportEntry);
            
            if (exportEntry != null)
            {
                return exportEntry.ExportObject ?? LoadExport(exportEntry.TableEntryIndex);
            }

            /*int exportArrayIndex = exportEntry?.TableEntryIndex ?? -1;
            UObject obj = null;
            bool found = false;

            if (exportArrayIndex >= 0)
            {
                obj = ExportedObjects[exportArrayIndex];
                int depth = 1;
                found = true;

                while (depth != pathParts.Length)
                {
                    obj = obj.InnerObjects.FirstOrDefault(inner => inner.ObjectName == pathParts[depth]);
                    depth++;

                    if (obj == null)
                    {
                        found = false;
                        break;
                    }
                }
            }

            if (found)
            {
                return obj;
            }*/

            // Some objects are implicitly exported under the archive's name and don't have a top level UPackage to contain
            // them, so try again with the first part of the path stripped if that might be the case here
            if (path.StartsWith(archivePathPrefix))
            {
                path = path.Substring(archivePathPrefix.Length);
                return GetExportedObjectByPath(path);
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
        /// Gets the export table entry for an object, withref requiring the exported object to be present.
        /// Useful when creating an archive in-memory; sometimes we need to reference an export before the
        /// actual object is available.
        /// </summary>
        /// <param name="fullObjectPath"></param>
        /// <returns>The table entry if found, or null otherwise.</returns>
        public FExportTableEntry GetExportTableEntry(string fullObjectPath)
        {
            if (ExportTableByObjectPath.TryGetValue(fullObjectPath, out var value))
            {
                return value;
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
        public UObject GetObjectByIndex(int index)
        {
            if (index == 0)
            {
                return null;
            }

            if (index < 0)
            {
                int importTableIndex = -1 * (index + 1);
                return ParentLinker.GetCookedObjectByPath(ImportTable[importTableIndex].FullObjectPath);
            }

            int exportTableIndex = index - 1;

            if (exportTableIndex >= ExportedObjects.Count)
            {
                throw new ArgumentException($"{nameof(GetObjectByIndex)}: requested index {index} ref of bounds. Only {ExportedObjects.Count} export objects exist in archive {FileName}.");
            }

            if (ExportedObjects[exportTableIndex] == null && IsLoading)
            {
                lock (_stream)
                {
                    long previousPosition = _stream.Position;
                    ExportedObjects[exportTableIndex] = LoadExport(exportTableIndex);
                    _stream.Position = previousPosition;
                }
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
                UObject outer = ExportedObjects[i].Outer;

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
                case "Core.Enum":
                case "Core.FloatProperty":
                case "Core.Function":
                case "Core.InterfaceProperty":
                case "Core.IntProperty":
                case "Core.NameProperty":
                case "Core.ObjectProperty":
                case "Core.ScriptStruct":
                case "Core.StrProperty":
                case "Core.StructProperty":
                // Miscellaneous
                case "Core.MetaData":
                case "Core.ObjectRedirector": // these should probably never show up in cooked packages
                case "Core.System":
                case "Core.TextBuffer":
                case "Engine.ActorChannel":
                case "Engine.ChannelDownload":
                case "Engine.ChildConnection":
                case "Engine.CodecMovieBink":
                case "Engine.ControlChannel":
                case "Engine.FileChannel":
                case "Engine.FracturedStaticMesh":
                case "Engine.GuidCache":
                case "Engine.Level":
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
            lock (_stream)
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
                    Log.Warning($"In archive {FileName}, object {exportObj.FullObjectPath} did not fully deserialize its data ({extraBytes} bytes remaining). Class is {ExportTable[i].ClassName}");
                }
#endif

                return exportObj;
            }
        }

        /// <summary>
        /// Reads the map of export objects to their imported dependencies.
        /// </summary>
        private void SerializeDependsMap()
        {
            if (!HasDependsMap)
            {
                return;
            }

            _stream.Seek(PackageFileSummary.DependsOffset, SeekOrigin.Begin);

            for (int i = 0; i < DependsMap.Length; i++)
            {
                _stream.Int32Array(ref DependsMap[i]);
            }
        }

        /// <summary>
        /// Deserializes all of the export objects which are contained in this archive.
        /// </summary>
        private void SerializeExportObjects(ProgressBar progressBar)
        {
            int numAlreadyLoaded = 0, numFailed = 0, numSucceeded = 0;

            for (int i = 0; i < ExportTable.Count; i++)
            {
                progressBar.Update("", i, ExportTable.Count);

                // This export may have been preloaded by another one - skip it if so
                if (ExportedObjects[i] != null)
                {
                    numAlreadyLoaded++;
                    continue;
                }

                try
                {
                    ExportedObjects[i] = LoadExport(i);
                    numSucceeded++;
                }
                catch (Exception e)
                {
                    numFailed++;
                    break; // TODO remove
                }
            }

            progressBar.Update("complete", ExportTable.Count, ExportTable.Count);

            if (numFailed > 0)
            {
                Log.Info($"Archive {FileName}: done reading export objects. {numSucceeded} succeeded and {numFailed} failed deserialization. {numAlreadyLoaded} were previously loaded.");
            }

            _stream.Close();
        }

        /// <summary>
        /// Serializes the metadata at the beginning of the package file, which describes version info, package flags, and the
        /// structure of the remainder of the package header.
        /// </summary>
        private void SerializeFileSummary()
        {
            _stream.UInt32(ref PackageFileSummary.Signature);

            if (IsLoading && PackageFileSummary.Signature != UNREAL_SIGNATURE)
            {
                throw new Exception("Package is expected to start with the Unreal package signature, 0x9E2A83C1");
            }

            _stream.UInt16(ref PackageFileSummary.FileVersion);
            _stream.UInt16(ref PackageFileSummary.LicenseeVersion);
            _stream.Int32(ref PackageFileSummary.HeaderSize);
            _stream.String(ref PackageFileSummary.FolderName);
            _stream.Enum32(ref PackageFileSummary.PackageFlags);
            _stream.Int32(ref PackageFileSummary.NameCount);
            _stream.Int32(ref PackageFileSummary.NameOffset);
            _stream.Int32(ref PackageFileSummary.ExportCount);
            _stream.Int32(ref PackageFileSummary.ExportOffset);
            _stream.Int32(ref PackageFileSummary.ImportCount);
            _stream.Int32(ref PackageFileSummary.ImportOffset);
            _stream.Int32(ref PackageFileSummary.DependsOffset);

            // The thumbnail table offset is just here for posterity; XCOM EW is cooked for console,
            // and the thumbnail table is gone. For some reason, the thumbnail table offset is still
            // set, even though it should be set to 0 in a cooked build. 
            _stream.Int32(ref PackageFileSummary.ThumbnailTableOffset);

            // 12 bytes of unknown data to get past
            if (IsLoading)
            {
                _stream.SkipBytes(12);
            }
            else
            {
                int unknownBytes = 0;
                _stream.Int32(ref unknownBytes);
                _stream.Int32(ref unknownBytes);
                _stream.Int32(ref unknownBytes);
            }

            _stream.Guid(ref PackageFileSummary.PackageGuid);
            _stream.Array(ref PackageFileSummary.Generations);
            _stream.Int32(ref PackageFileSummary.EngineVersion);
            _stream.Int32(ref PackageFileSummary.CookerVersion);
            _stream.Enum32(ref PackageFileSummary.CompressionFlags);
            _stream.Int32(ref PackageFileSummary.NumCompressedChunks);

            // We aren't going to do decompression, so just bail
            if (PackageFileSummary.NumCompressedChunks != 0)
            {
                _stream.Close();
                return;
            }

            _stream.UInt32(ref PackageFileSummary.PackageSource);
            _stream.StringArray(ref PackageFileSummary.AdditionalPackagesToCook);

#if DEBUG
            if (IsLoading)
            {
                if (PackageFileSummary.FileVersion != 845 || PackageFileSummary.LicenseeVersion != 64)
                {
                    Log.Warning($"Expected archive {FileName} to be version 845/64, but it's actually {PackageFileSummary.FileVersion}/{PackageFileSummary.LicenseeVersion}");
                }
            }
#endif
        }

        private void SerializeExportTable()
        {
            ExportTable = new List<FExportTableEntry>(new FExportTableEntry[PackageFileSummary.ExportCount]);

            if (PackageFileSummary.ExportCount == 0)
            {
                return;
            }

            _stream.Seek(PackageFileSummary.ExportOffset, SeekOrigin.Begin);

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
                    ExportTableByObjectPath[ExportTable[i].FullObjectPath] = ExportTable[i];
                }
            }
        }

        private void SerializeImportTable()
        {
            ImportTable = new List<FImportTableEntry>(new FImportTableEntry[PackageFileSummary.ImportCount]);

            if (PackageFileSummary.ImportCount == 0)
            {
                return;
            }

            _stream.Seek(PackageFileSummary.ImportOffset, SeekOrigin.Begin);

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
            NameTable = new List<string>(new string[PackageFileSummary.NameCount]);

            if (PackageFileSummary.NameCount == 0)
            {
                return;
            }

            _stream.Seek(PackageFileSummary.NameOffset, SeekOrigin.Begin);

            for (int i = 0; i < PackageFileSummary.NameCount; i++)
            {
                // Weird use of a local variable to make this work for both reading and writing
                string name = NameTable[i];
                _stream.String(ref name);
                NameTable[i] = name;

                // Every name has a 64-bit object flag field, but we don't care abref them
                _stream.SkipBytes(8);
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
