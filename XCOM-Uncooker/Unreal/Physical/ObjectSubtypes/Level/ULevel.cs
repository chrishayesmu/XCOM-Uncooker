﻿using System;
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
            stream.Object(ref Scale3D);
            stream.Int32(ref CachedDataIndex);
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
            stream.Object(ref Scale3D);
            stream.Int32(ref CachedDataIndex);
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
            stream.Int32(ref ActorRefItem);
            stream.UInt8(ref SlotIdx);
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
            stream.Object(ref BoundingSphere);
            stream.Float32(ref TexelFactor);
            stream.Int32(ref Texture);
            stream.BoolAsInt32(ref bAttached);
            stream.Float32(ref OriginalRadius);
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
            stream.Guid(ref Guid);
            stream.Int32(ref RefId);
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
            stream.BoolAsInt32(ref bInitialized);

            if (bInitialized)
            {
                stream.Object(ref Bounds);
                stream.Float32(ref SampleSpacing);
                stream.Array(ref Samples);
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
            stream.Int32(ref CellDataSize);
            stream.Array(ref Cells);
            stream.Array(ref CellDataChunks);
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
            stream.Object(ref Min);
            stream.Int16(ref ChunkIndex);
            stream.Int16(ref DataOffset);
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
            stream.BoolAsInt32(ref bCompressed);
            stream.Int32(ref UncompressedSize);
            stream.ByteArray(ref Data);
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
            stream.Object(ref PrecomputedVisibilityCellBucketOriginXY);
            stream.Float32(ref PrecomputedVisibilityCellSizeXY);
            stream.Float32(ref PrecomputedVisibilityCellSizeZ);
            stream.Int32(ref PrecomputedVisibilityCellBucketSizeXY);
            stream.Int32(ref PrecomputedVisibilityNumCellBuckets);
            stream.Array(ref PrecomputedVisibilityCellBuckets);
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
            stream.Float32(ref VolumeMaxDistance);
            stream.Object(ref VolumeBox);
            stream.Int32(ref VolumeSizeX);
            stream.Int32(ref VolumeSizeY);
            stream.Int32(ref VolumeSizeZ);
            stream.Array(ref Data);
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
            stream.Object(ref BoundingSphere);
            stream.Float32(ref TexelFactor);
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
            stream.Object(ref Position);
            stream.Float32(ref Radius);
            stream.UInt8(ref IndirectDirectionTheta);
            stream.UInt8(ref IndirectDirectionPhi);
            stream.UInt8(ref EnvironmentDirectionTheta);
            stream.UInt8(ref EnvironmentDirectionPhi);
            stream.Object(ref IndirectRadiance);
            stream.Object(ref EnvironmentRadiance);
            stream.Object(ref AmbientRadiance);
            stream.UInt8(ref bShadowedFromDominantLights);
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

            stream.Int32(ref Model);
            stream.Int32Array(ref ModelComponents);
            stream.Int32Array(ref GameSequences);
            stream.Map(ref TextureToInstancesMap);
            stream.Map(ref DynamicTextureInstances);
            stream.ByteArray(ref ApexData);
            stream.BulkArray(ref CachedPhysBSPData);
            stream.MultiMap(ref CachedPhysSMDataMap);
            stream.Array(ref CachedPhysSMDataStore);
            stream.MultiMap(ref CachedPhysPerTriSMDataMap);
            stream.Array(ref CachedPhysPerTriSMDataStore);
            stream.Int32(ref CachedPhysBSPDataVersion);
            stream.Int32(ref CachedPhysSMDataVersion);
            stream.Map(ref ForceStreamTextures);
            stream.Object(ref CachedPhysConvexBSPData);
            stream.Int32(ref CachedPhysConvexBSPVersion);
            stream.Int32(ref NavListStart);
            stream.Int32(ref NavListEnd);
            stream.Int32(ref CoverListStart);
            stream.Int32(ref CoverListEnd);
            stream.Int32(ref PylonListStart);
            stream.Int32(ref PylonListEnd);
            stream.Array(ref CrossLevelCoverGuidRefs);
            stream.Int32Array(ref CoverLinkRefs);
            stream.Array(ref CoverIndexPairs);
            stream.Int32Array(ref CrossLevelActors);
            stream.Object(ref PrecomputedLightVolume);
            stream.Object(ref PrecomputedVisibilityHandler);
            stream.Object(ref PrecomputedVolumeDistanceField);
        }
    }
}
