using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Textures;

namespace UnrealArchiveLibrary.Unreal
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

        public void PopulateDependencies(List<int> dependencyIndices)
        {
            foreach (var textureType in TextureTypes)
            {
                textureType.PopulateDependencies(dependencyIndices);
            }
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

        [Index(typeof(UTexture))]
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

            // These indices should theoretically be adjusted on clone; actually, all of the texture types should be
            // discarded and calculated from scratch for each archive, since they depend on which textures make it into
            // the uncooked archive. That's not happening yet, and in fact these aren't even used when uncooking, so
            // we're just copying the indices over out of sheer laziness.
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

        public void PopulateDependencies(List<int> dependencyIndices)
        {
            dependencyIndices.AddRange(ExportIndices);
        }
    }

    /// <summary>
    /// The package file summary is the very beginning of an Unreal package file archive. It contains
    /// metadata regarding the package as a whole, as well as the package header specifically.
    /// (The package header includes this summary, the name/import/export tables, and the
    /// thumbnail metadata table.)
    /// </summary>
    public struct FPackageFileSummary : IUnrealSerializable
    {
        public FPackageFileSummary() { }

        #region Serialized data

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
        public FCompressedChunk[] CompressedChunks;
        public uint PackageSource;
        public string[] AdditionalPackagesToCook;
        public FTextureAllocations TextureAllocations;

        #endregion

        // Whether this file summary is currently being copied from a compressed archive to a decompressed one.
        // Affects how the summary writes to disk.
        public bool IsCopyingForDecompression = false;

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FPackageFileSummary) sourceObj;

            Signature = other.Signature;
            PackageName = other.PackageName;
            FileVersion = other.FileVersion;
            LicenseeVersion = other.LicenseeVersion;
            HeaderSize = other.HeaderSize;
            FolderName = other.FolderName;
            PackageFlags = other.PackageFlags;
            NameCount = other.NameCount;
            NameOffset = other.NameOffset;
            ExportCount = other.ExportCount;
            ExportOffset = other.ExportOffset;
            ImportCount = other.ImportCount;
            ImportOffset = other.ImportOffset;
            DependsOffset = other.DependsOffset;
            ImportExportGuidsOffset = other.ImportExportGuidsOffset;
            ImportGuidsCount = other.ImportGuidsCount;
            ExportGuidsCount = other.ExportGuidsCount;
            ThumbnailTableOffset = other.ThumbnailTableOffset;
            PackageGuid = other.PackageGuid;
            Generations = other.Generations;
            EngineVersion = other.EngineVersion;
            CookerVersion = other.CookerVersion;
            CompressionFlags = other.CompressionFlags;
            CompressedChunks = other.CompressedChunks;
            PackageSource = other.PackageSource;
            AdditionalPackagesToCook = other.AdditionalPackagesToCook;
            TextureAllocations = other.TextureAllocations;
        }

        public void Serialize(IUnrealDataStream stream)
        {
            var archive = stream.Archive;

            if (stream.IsWrite && !IsCopyingForDecompression)
            {
                // Remap the net indices of objects so they're contiguous; this doesn't really matter, but it does make
                // the UDK spit out a lot less warning logs, which is nice
                int lastNetIndex = 0;

                for (int i = 0; i < archive.ExportedObjects.Length; i++)
                {
                    // FIXME: there's a bug causing ExportedObjects to be bigger than ExportTable somewhere
                    if (archive.ExportedObjects[i] == null)
                    {
                        continue;
                    }

                    if (archive.ExportedObjects[i].NetIndex != 0)
                    {
                        archive.ExportedObjects[i].NetIndex = lastNetIndex++;
                    }
                }

                // Before we can serialize the file summary to disk, we need to populate the generation data.
                // We always have a single generation.
                Generations[0].ExportCount = archive.ExportTable.Count;
                Generations[0].NameCount = archive.NameTable.Count;
                Generations[0].NetObjectCount = lastNetIndex;
            }

            stream.UInt32(ref Signature);

            if (stream.IsRead && Signature != FArchive.UNREAL_SIGNATURE)
            {
                throw new Exception("Package is expected to start with the Unreal package signature, 0x9E2A83C1");
            }

            stream.UInt16(ref FileVersion);
            stream.UInt16(ref LicenseeVersion);

            // If the file version doesn't match, either the UPK isn't from XCOM, or it's actually a fully-compressed
            // file and we're reading compression metadata rather than the summary data. Check for the latter.
            if (stream.IsRead && FileVersion != 845)
            {
                stream.Seek(4, SeekOrigin.Begin);

                int chunkSize = 0, compressedSize = 0, uncompressedSize = 0;
                stream.Int32(ref chunkSize);
                stream.Int32(ref compressedSize);
                stream.Int32(ref uncompressedSize);

                // The right thing to do would be to compare the actual file size with the one we'd expect based on
                // the compression metadata, but I'm lazy and this basic sanity check will suffice for XCOM
                if (chunkSize == 131072 && compressedSize < uncompressedSize)
                {
                    PackageFlags |= PackageFlag.StoreFullyCompressed;
                    return;
                }
                else
                {
                    throw new Exception($"Archive {archive.FileName} has an invalid file version: {FileVersion}");
                }
            }

            stream.Int32(ref HeaderSize);
            stream.String(ref FolderName);
            stream.Enum32(ref PackageFlags);
            stream.Int32(ref NameCount);
            stream.Int32(ref NameOffset);
            stream.Int32(ref ExportCount);
            stream.Int32(ref ExportOffset);
            stream.Int32(ref ImportCount);
            stream.Int32(ref ImportOffset);
            stream.Int32(ref DependsOffset);
            stream.Int32(ref ImportExportGuidsOffset);
            stream.Int32(ref ImportGuidsCount);
            stream.Int32(ref ExportGuidsCount);

            // The thumbnail table offset is just here for posterity; XCOM EW is cooked for console,
            // and the thumbnail table is gone. For some reason, the thumbnail table offset is still
            // set, even though it should be set to 0 in a cooked build. 
            stream.Int32(ref ThumbnailTableOffset);

            stream.Guid(ref PackageGuid);
            stream.Array(ref Generations);
            stream.Int32(ref EngineVersion);
            stream.Int32(ref CookerVersion);
            stream.Enum32(ref CompressionFlags);
            stream.Array(ref CompressedChunks);
            stream.UInt32(ref PackageSource);
            stream.StringArray(ref AdditionalPackagesToCook);
            stream.Object(ref TextureAllocations);

            // UPKUtils doesn't remove the StoreFullyCompressed flag from packages it decompresses, which doesn't seem to
            // have any runtime impact on the game, but it messes us up. Clear it out or uncooking modded games will fail
            PackageFlags &= ~PackageFlag.StoreFullyCompressed;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
            TextureAllocations.PopulateDependencies(dependencyIndices);
        }
    }
}
