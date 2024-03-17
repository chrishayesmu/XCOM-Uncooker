using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCOM_Uncooker.Unreal.Physical
{
    /// <summary>
    /// The package file summary is the very beginning of an Unreal package file archive. It contains
    /// metadata regarding the package as a whole, as well as the package header specifically.
    /// (The package header includes this summary, the name/import/export tables, and the
    /// thumbnail metadata table.)
    /// </summary>
    public struct FPackageFileSummary
    {
        public uint Signature;
        public string PackageName;
        public ushort FileVersion;
        public ushort LicenseeVersion;
        public int HeaderSize;
        public string FolderName;
        public PackageFlag PackageFlags;
        public int NameCount;
        public int NameOffset;
        public int ExportCount;
        public int ExportOffset;
        public int ImportCount;
        public int ImportOffset;
        public int DependsOffset;
        public int ThumbnailTableOffset;
        public Guid PackageGuid;
        public FGenerationInfo[] Generations;
        public int EngineVersion;
        public int CookerVersion;
        public CompressionFlag CompressionFlags;
        public int NumCompressedChunks;
        public uint PackageSource;
        public string[] AdditionalPackagesToCook;
    }
}
