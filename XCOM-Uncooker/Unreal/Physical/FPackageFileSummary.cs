using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical
{
    public struct FTextureAllocations : IUnrealSerializable
    {
        public FTextureAllocations() {}

        #region Serialized data

        public FTextureType[] TextureTypes = [];

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FTextureAllocations) sourceObj;

            TextureTypes = other.TextureTypes;
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Array(ref TextureTypes);
        }
    }

    public struct FTextureType : IUnrealSerializable
    {
        #region Serialized data

        public int SizeX;

        public int SizeY;

        public int NumMips;

        public uint Format;

        public uint TexCreateFlags;

        // TODO: this may be export table indices, which would need to be fixed up
        public int[] ExportIndices;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FTextureType) sourceObj;

            SizeX = other.SizeX;
            SizeY = other.SizeY;
            NumMips = other.NumMips;
            Format = other.Format;
            TexCreateFlags = other.TexCreateFlags;
            ExportIndices = other.ExportIndices;
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref SizeX);
            stream.Int32(ref SizeY);
            stream.Int32(ref NumMips);
            stream.UInt32(ref Format);
            stream.UInt32(ref TexCreateFlags);
            stream.Int32Array(ref ExportIndices);
        }
    }

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
        public int ImportExportGuidsOffset;
        public int ImportGuidsCount;
        public int ExportGuidsCount;
        public int ThumbnailTableOffset;
        public Guid PackageGuid;
        public FGenerationInfo[] Generations;
        public int EngineVersion;
        public int CookerVersion;
        public CompressionFlag CompressionFlags;
        public int NumCompressedChunks;
        public uint PackageSource;
        public string[] AdditionalPackagesToCook;
        public FTextureAllocations TextureAllocations;
    }
}
