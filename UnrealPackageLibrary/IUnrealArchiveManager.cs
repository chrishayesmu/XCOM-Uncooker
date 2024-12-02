using UnrealArchiveLibrary.Unreal;

namespace UnrealPackageLibrary
{
    /// <summary>
    /// Used to define in which circumstances dependencies should automatically be loaded.
    /// </summary>
    public enum DependencyLoadingMode
    {
        /// <summary>
        /// Never load dependencies automatically; only use archives specified explicitly.
        /// </summary>
        None,

        /// <summary>
        /// Automatically load dependencies for type data, such as classes and structs. Note that if an
        /// archive is loaded for its type data, its other exports will also become available for use.
        /// </summary>
        TypesOnly,

        /// <summary>
        /// Automatically load all dependencies, regardless of their type.
        /// </summary>
        All
    }

    public enum ArchiveFormat
    {
        XComEW,
        XCom2WotC,
        Unknown
    }

    public enum ProgressEvent
    {
        ArchiveHeaderLoaded,
        ArchiveBodyLoaded,
        ArchiveUncookedInMemory,
        ArchivePostUncookFixup, // After initial uncooking, some things are fixed up that can only be handled once all objects are available.
        ArchiveWrittenToDisk,
        DependencyLoaded, // An archive which is a dependency of another archive has been loaded (up to its headers).
        LoadComplete,
        UncookComplete, // All archives are uncooked in memory (but not necessarily written to disk).
    }

    /// <summary>
    /// Used for reporting progress on various archive tasks.
    /// </summary>
    /// <param name="e">What type of event is being reported on.</param>
    /// <param name="numCompleted">How many of this type of event are complete.</param>
    /// <param name="numTotal">How many total of this type of event are anticipated.</param>
    public delegate void ProgressHandler(ProgressEvent e, int numCompleted, int numTotal);

    public struct ArchiveManagerSettings
    {
        public ArchiveFormat OutputFormat = ArchiveFormat.XComEW;

        /// <summary>
        /// The maximum degree of parallelism to use when decompressing archives. Since the decompression and subsequent
        /// writing to disk are fairly intensive, setting this too high is likely to degrade performance.
        /// </summary>
        public int MaxParallelismForDecompression = 4;

        /// <summary>
        /// The maximum degree of parallelism to use when serializing or deserializing archives.
        /// </summary>
        public int MaxParallelismForSerialization = 100;

        /// <summary>
        /// The maximum degree of parallelism to use when uncooking archives.
        /// </summary>
        public int MaxParallelismForUncooking = 100;

        /// <summary>
        /// The file extensions which should be considered to represent Unreal archive files.
        /// </summary>
        public List<string> ArchiveFileExtensions = [ ".u", ".udk", ".umap", ".upk", ".xxx" ];

        public ArchiveManagerSettings()
        {
        }
    }

    public struct TextureFileCacheEntry
    {
        public string TextureFileName;

        public string FilePath;
    }

    /// <summary>
    /// <para>
    /// Core entry point to the UnrealPackageLibrary. An <c>IUnrealArchiveManager</c> is a stateful
    /// class which reads in a set of input archives and uses their data when making any changes.
    /// For example, to uncook an asset archive, the <c>IUnrealArchiveManager</c> will likely need
    /// the <c>Core</c> and <c>Engine</c> UPKs in order to look up classes referenced by the asset.
    /// Additional UPKs may also be needed, such as if game-specific classes are in use.
    /// </para>
    /// <para>
    /// The intended flow of this class is that archives will first be loaded in using <see cref="LoadInputArchives(IEnumerable{string})"/>
    /// or one of its overloads. Archives loaded this way are considered read-only, and should not be
    /// modified directly. Instead, copies should be made using <see cref="CloneArchives(IEnumerable{FArchive}, Linker?)"/>
    /// or <see cref="UncookArchives(IEnumerable{FArchive})"/>. Those copies can then be changed freely, 
    /// either directly or using other functions in this interface.
    /// </para>
    /// </summary>
    public interface IUnrealArchiveManager : IDisposable
    {
        /// <summary>
        /// Settings for configuring the behavior of this <see cref="IUnrealArchiveManager"/>.
        /// </summary>
        public abstract ArchiveManagerSettings Settings { get; }

        /// <summary>
        /// A <see cref="Linker"/> containing the set of archives which have been loaded as inputs.
        /// </summary>
        public Linker InputLinker { get; }

        /// <summary>
        /// Creates cloned copies of the given archives, which can then be modified freely.
        /// If <paramref name="linkerToUse"/> is not null, then the copied archives will be added to the
        /// set of archives owned by that linker; otherwise, a new linker is created.
        /// </summary>
        /// <param name="archives">The archives which should be cloned</param>
        /// <param name="linkerToUse">
        /// If provided, the cloned archives will be added to this linker's set of archives. Any archives
        /// which are already present in the linker <b>will not</b> be cloned.
        /// </param>
        /// <returns>The <see cref="Linker"/> containing the newly cloned archives.</returns>
        public Linker CloneArchives(IEnumerable<FArchive> archives, Linker? linkerToUse = null);

        /// <summary>
        /// Gets the set of "logical" archives contained in the input archives. A cooked archive can contain
        /// assets from multiple sources, which prior to cooking, would have been separate archives. The logical
        /// archives comprise the set of what the original, pre-cooking archives would have been.
        /// </summary>
        /// <remarks>
        /// If none of the input archives are cooked, then the result will just be the set of input archive names.
        /// </remarks>
        /// <param name="archives">
        /// If set, then instead of pulling from the complete set of input archives which have been loaded via this
        /// interface, the logical archives will be based on this set.
        /// </param>
        /// <returns></returns>
        public ISet<string> GetLogicalArchivesFromInputs(IEnumerable<FArchive>? archives = null);

        /// <summary>
        /// Loads all archives from the paths provided. Each path should be to a file; directories will
        /// result in an error. This function will load only the archives requested, and will not attempt
        /// to automatically locate any missing dependencies. If that is desired, then instead use
        /// <see cref="LoadInputArchives(string, IEnumerable{string}, DependencyLoadingMode)"/>
        /// </summary>
        /// <param name="inputArchivePaths">Paths to the archive files to load</param>
        public void LoadInputArchives(IEnumerable<string> inputArchivePaths, ProgressHandler? progressHandler = null);

        /// <summary>
        /// Loads all archives from the paths provided. Each path should be to a file; directories will
        /// result in an error. If an archive depends on objects defined elsewhere, and the depended-on archive
        /// is not part of the requested set, then it may be loaded automatically, depending on the value of
        /// <paramref name="dependencyMode"/>.
        /// </summary>
        /// <param name="baseDirectory">When automatically loading dependencies, it will be assumed that they can be found
        /// in this directory. Subdirectories will be included in the search.</param>
        /// <param name="inputArchivePaths">Paths to the archive files to load</param>
        /// <param name="dependencyMode">How to handle missing dependencies during loading</param>
        public void LoadInputArchives(string baseDirectory, IEnumerable<string> inputArchivePaths, ProgressHandler? progressHandler = null, DependencyLoadingMode dependencyMode = DependencyLoadingMode.All);

        /// <summary>
        /// Creates uncooked archives based on the contents of the currently loaded input archives.  Uncooked archives
        /// exist only in memory and are not automatically serialized to disk.
        /// </summary>
        /// <param name="textureFileCacheEntries">
        /// If set, texture data will be pulled from the given files.
        /// </param>
        /// <param name="inputArchivesOverride">
        /// If set, then instead of using all currently loaded input archives, uncooking will be based on the set of archives provided.
        /// </param>
        /// <param name="outputArchivesOverride">
        /// If set, then only archives matching these names will be kept in the uncooked output.
        /// </param>
        /// <returns></returns>
        public Linker UncookArchives(IEnumerable<TextureFileCacheEntry>? textureFileCacheEntries = null, ProgressHandler? progressHandler = null, IEnumerable<FArchive>? inputArchivesOverride = null, ISet<string>? outputArchivesOverride = null);

        /// <summary>
        /// Writes the given archive to a file on disk.
        /// </summary>
        /// <param name="archive">The archive to write.</param>
        /// <param name="containingFolderPath">The folder path the file will be at, not including the filename. Any existing file will be overwritten.</param>
        public void WriteArchiveToDisk(FArchive archive, string containingFolderPath);
    }
}
