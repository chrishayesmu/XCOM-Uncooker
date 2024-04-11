using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Components;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Models;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Physics;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Textures;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Level
{
    [FixedSize(16)]
    public struct FCachedPerTriPhysSMData : IUnrealSerializable
    {
        #region Serialized data

        public FVector Scale3D;

        public int CachedDataIndex;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(out Scale3D);
            stream.Int32(out CachedDataIndex);
        }
    }

    [FixedSize(16)]
    public struct FCachedPhysSMData : IUnrealSerializable
    {
        #region Serialized data

        public FVector Scale3D;

        public int CachedDataIndex;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(out Scale3D);
            stream.Int32(out CachedDataIndex);
        }
    }

    [FixedSize(5)]
    public struct FCoverIndexPair : IUnrealSerializable
    {
        #region Serialized data

        public int ActorRefItem;

        public byte SlotIdx;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(out ActorRefItem);
            stream.UInt8(out SlotIdx);
        }
    }

    [FixedSize(32)]
    public struct FDynamicTextureInstance : IUnrealSerializable
    {
        #region Serialized data

        public FSphere BoundingSphere;

        public float TexelFactor;

        [Index(typeof(UTexture2D))]
        public int Texture;

        public bool bAttached; // UBOOL

        public float OriginalRadius;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(out BoundingSphere);
            stream.Float32(out TexelFactor);
            stream.Int32(out Texture);
            stream.BoolAsInt32(out bAttached);
            stream.Float32(out OriginalRadius);
        }
    }

    [FixedSize(20)]
    public struct FGuidPair : IUnrealSerializable
    {
        #region Serialized data

        public Guid Guid;

        public int RefId;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Guid(out Guid);
            stream.Int32(out RefId);
        }
    }

    public struct FPrecomputedLightVolume : IUnrealSerializable
    {
        #region Serialized data

        public bool bInitialized; // UBOOL

        public FBox Bounds;

        public float SampleSpacing;

        public FVolumeLightingSample[] Samples;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.BoolAsInt32(out bInitialized);

            if (bInitialized)
            {
                stream.Object(out Bounds);
                stream.Float32(out SampleSpacing);
                stream.Array(out Samples);
            }
        }
    }

    public struct FPrecomputedVisibilityBucket : IUnrealSerializable
    {
        #region Serialized data

        public int CellDataSize;

        public FPrecomputedVisibilityCell[] Cells;

        public FCompressedVisibilityChunk[] CellDataChunks;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(out CellDataSize);
            stream.Array(out Cells);
            stream.Array(out CellDataChunks);
        }
    }

    public struct FPrecomputedVisibilityCell : IUnrealSerializable
    {
        #region Serialized data

        public FVector Min;

        public short ChunkIndex;

        public short DataOffset;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(out Min);
            stream.Int16(out ChunkIndex);
            stream.Int16(out DataOffset);
        }
    }

    public struct FCompressedVisibilityChunk : IUnrealSerializable
    {
        #region Serialized data

        public bool bCompressed; // UBOOL

        public int UncompressedSize;

        public byte[] Data;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.BoolAsInt32(out bCompressed);
            stream.Int32(out UncompressedSize);
            stream.ByteArray(out Data);
        }
    }

    public struct FPrecomputedVisibilityHandler : IUnrealSerializable
    {
        #region Serialized data

        public FVector2D PrecomputedVisibilityCellBucketOriginXY;

        public float PrecomputedVisibilityCellSizeXY;

        public float PrecomputedVisibilityCellSizeZ;

        public int PrecomputedVisibilityCellBucketSizeXY;

        public int PrecomputedVisibilityNumCellBuckets;

        public FPrecomputedVisibilityBucket[] PrecomputedVisibilityCellBuckets;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(out PrecomputedVisibilityCellBucketOriginXY);
            stream.Float32(out PrecomputedVisibilityCellSizeXY);
            stream.Float32(out PrecomputedVisibilityCellSizeZ);
            stream.Int32(out PrecomputedVisibilityCellBucketSizeXY);
            stream.Int32(out PrecomputedVisibilityNumCellBuckets);
            stream.Array(out PrecomputedVisibilityCellBuckets);
        }
    }

    public struct FPrecomputedVolumeDistanceField : IUnrealSerializable
    {
        #region Serialized data

        public float VolumeMaxDistance;

        public FBox VolumeBox;

        public int VolumeSizeX;

        public int VolumeSizeY;

        public int VolumeSizeZ;

        public FColor[] Data;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Float32(out VolumeMaxDistance);
            stream.Object(out VolumeBox);
            stream.Int32(out VolumeSizeX);
            stream.Int32(out VolumeSizeY);
            stream.Int32(out VolumeSizeZ);
            stream.Array(out Data);
        }
    }

    [FixedSize(20)]
    public struct FStreamableTextureInstance : IUnrealSerializable
    {
        #region Serialized data

        public FSphere BoundingSphere;

        public float TexelFactor;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(out BoundingSphere);
            stream.Float32(out TexelFactor);
        }
    }

    [FixedSize(35)]
    public struct FVolumeLightingSample : IUnrealSerializable
    {
        #region Serialized data

        public FVector Position;

        public float Radius;

        public byte IndirectDirectionTheta;

        public byte IndirectDirectionPhi;

        public byte EnvironmentDirectionTheta;

        public byte EnvironmentDirectionPhi;

        public FColor IndirectRadiance;

        public FColor EnvironmentRadiance;

        public FColor AmbientRadiance;

        public byte bShadowedFromDominantLights;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(out Position);
            stream.Float32(out Radius);
            stream.UInt8(out IndirectDirectionTheta);
            stream.UInt8(out IndirectDirectionPhi);
            stream.UInt8(out EnvironmentDirectionTheta);
            stream.UInt8(out EnvironmentDirectionPhi);
            stream.Object(out IndirectRadiance);
            stream.Object(out EnvironmentRadiance);
            stream.Object(out AmbientRadiance);
            stream.UInt8(out bShadowedFromDominantLights);
        }
    }

    public class ULevel(FArchive archive, FObjectTableEntry tableEntry) : ULevelBase(archive, tableEntry)
    {
        #region Serialized data

        [Index(typeof(UModel))]
        public int Model;

        [Index(typeof(UModelComponent))]
        public int[] ModelComponents;

        [Index(typeof(UObject))]
        public int[] GameSequences;

        [Index(typeof(UTexture2D))]
        public IDictionary<int, FStreamableTextureInstance[]> TextureToInstancesMap;

        public IDictionary<int, FDynamicTextureInstance[]> DynamicTextureInstances;

        public byte[] ApexData;

        public byte[] CachedPhysBSPData; // bulk

        [Index(typeof(UStaticMesh))]
        public IDictionary<int, IList<FCachedPhysSMData>> CachedPhysSMDataMap;

        public FKCachedConvexData[] CachedPhysSMDataStore;

        [Index(typeof(UStaticMesh))]
        public IDictionary<int, IList<FCachedPerTriPhysSMData>> CachedPhysPerTriSMDataMap;

        public FKCachedPerTriData[] CachedPhysPerTriSMDataStore;

        public int CachedPhysBSPDataVersion;

        public int CachedPhysSMDataVersion;

        [Index(typeof(UTexture2D))]
        public IDictionary<int, bool> ForceStreamTextures; // value is UBOOL

        public FKCachedConvexData CachedPhysConvexBSPData;

        public int CachedPhysConvexBSPVersion;

        [Index(typeof(UObject))]
        public int NavListStart;

        [Index(typeof(UObject))]
        public int NavListEnd;

        [Index(typeof(UObject))]
        public int CoverListStart;

        [Index(typeof(UObject))]
        public int CoverListEnd;

        [Index(typeof(UObject))]
        public int PylonListStart;

        [Index(typeof(UObject))]
        public int PylonListEnd;

        public FGuidPair[] CrossLevelCoverGuidRefs;

        [Index(typeof(UObject))]
        public int[] CoverLinkRefs;

        public FCoverIndexPair[] CoverIndexPairs;

        [Index(typeof(UObject))]
        public int[] CrossLevelActors;

        public FPrecomputedLightVolume PrecomputedLightVolume;

        public FPrecomputedVisibilityHandler PrecomputedVisibilityHandler;

        public FPrecomputedVolumeDistanceField PrecomputedVolumeDistanceField;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(out Model);
            stream.Int32Array(out ModelComponents);
            stream.Int32Array(out GameSequences);
            stream.Map(out TextureToInstancesMap);
            stream.Map(out DynamicTextureInstances);
            stream.ByteArray(out ApexData);
            stream.BulkArray(out CachedPhysBSPData);
            stream.MultiMap(out CachedPhysSMDataMap);
            stream.Array(out CachedPhysSMDataStore);
            stream.MultiMap(out CachedPhysPerTriSMDataMap);
            stream.Array(out CachedPhysPerTriSMDataStore);
            stream.Int32(out CachedPhysBSPDataVersion);
            stream.Int32(out CachedPhysSMDataVersion);
            stream.Map(out ForceStreamTextures);
            stream.Object(out CachedPhysConvexBSPData);
            stream.Int32(out CachedPhysConvexBSPVersion);
            stream.Int32(out NavListStart);
            stream.Int32(out NavListEnd);
            stream.Int32(out CoverListStart);
            stream.Int32(out CoverListEnd);
            stream.Int32(out PylonListStart);
            stream.Int32(out PylonListEnd);
            stream.Array(out CrossLevelCoverGuidRefs);
            stream.Int32Array(out CoverLinkRefs);
            stream.Array(out CoverIndexPairs);
            stream.Int32Array(out CrossLevelActors);
            stream.Object(out PrecomputedLightVolume);
            stream.Object(out PrecomputedVisibilityHandler);
            stream.Object(out PrecomputedVolumeDistanceField);
        }
    }
}
