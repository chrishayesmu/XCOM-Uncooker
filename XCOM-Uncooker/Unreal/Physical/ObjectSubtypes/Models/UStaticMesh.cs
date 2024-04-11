using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Components;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Physics;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Models
{
    public struct FFragmentRange : IUnrealSerializable
    {
        #region Serialized data

        public int BaseIndex;

        public int NumPrimitives;


        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(out BaseIndex);
            stream.Int32(out NumPrimitives);
        }
    }

    public struct FPositionVertexBuffer : IUnrealSerializable
    {
        #region Serialized data

        public uint Stride;

        public uint NumVertices;

        public byte[] VertexData;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.UInt32(out Stride);
            stream.UInt32(out NumVertices);
            stream.BulkArray(out VertexData);
        }
    }

    public struct FStaticMeshElement : IUnrealSerializable
    {
        #region Serialized data

        [Index(typeof(UObject))]
        public int Material;

        public bool EnableCollision; // UBOOL

        public bool OldEnableCollision; // UBOOL

        public bool bEnableShadowCasting; // UBOOL

        public uint FirstIndex;

        public uint NumTriangles;

        public uint MinVertexIndex;

        public uint MaxVertexIndex;

        public int MaterialIndex;

        public FFragmentRange[] Fragments;

        public bool LoadPlatformData;

        // public FPS3StaticMeshData PlatformData; // hopefully not needed?

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(out Material);
            stream.BoolAsInt32(out EnableCollision);
            stream.BoolAsInt32(out OldEnableCollision);
            stream.BoolAsInt32(out bEnableShadowCasting);
            stream.UInt32(out FirstIndex);
            stream.UInt32(out NumTriangles);
            stream.UInt32(out MinVertexIndex);
            stream.UInt32(out MaxVertexIndex);
            stream.Int32(out MaterialIndex);
            stream.Array(out Fragments);
            stream.Bool(out LoadPlatformData);

#if DEBUG
            if (LoadPlatformData)
            {
                Debugger.Break();
            }
#endif
        }
    }

    public struct FStaticMeshLODInfo : IUnrealSerializable
    {
        public void Serialize(IUnrealDataStream stream)
        {
            // Deliberate no-op; it seems like this is only "serialized" during internal operations, and
            // not to an archive file. Leaving this here to show that it's not an accidental omission.
        }
    }

    public struct FStaticMeshOptimizationSettings : IUnrealSerializable
    {
        #region Serialized data

        public float MaxDeviationPercentage;

        public byte SilhouetteImportance;
        
        public byte TextureImportance;

        public byte NormalMode;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            throw new NotImplementedException();
        }
    }

    public struct FStaticMeshRenderData : IUnrealSerializable
    {
        public FStaticMeshRenderData() {}

        #region Serialized data

        public FUntypedBulkData RawTriangles = new FUntypedBulkData();

        public FStaticMeshElement[] Elements;

        public FPositionVertexBuffer PositionVertexBuffer;

        public FStaticMeshVertexBuffer VertexBuffer;

        public FColorVertexBuffer ColorVertexBuffer;

        public uint NumVertices;

        public byte[] IndexBuffer;

        public byte[] WireframeIndexBuffer;

        public byte[] AdjacencyIndexBuffer;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            RawTriangles.Serialize(stream);
            stream.Array(out Elements);
            stream.Object(out PositionVertexBuffer);
            stream.Object(out VertexBuffer);
            stream.Object(out ColorVertexBuffer);
            stream.UInt32(out NumVertices);
            stream.BulkArray(out IndexBuffer);
            stream.BulkArray(out WireframeIndexBuffer);
            stream.BulkArray(out AdjacencyIndexBuffer);
        }
    }

    public struct FStaticMeshVertexBuffer : IUnrealSerializable
    {
        #region Serialized data

        public uint NumTexCoords;

        public uint Stride;

        public uint NumVertices;

        public bool bUseFullPrecisionUVs; // UBOOL

        public byte[] VertexData;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.UInt32(out NumTexCoords);
            stream.UInt32(out Stride);
            stream.UInt32(out NumVertices);
            stream.BoolAsInt32(out bUseFullPrecisionUVs);
            stream.BulkArray(out VertexData);
        }
    }

    public class UStaticMesh(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public FBoxSphereBounds Bounds;

        [Index(typeof(URB_BodySetup))]
        public int BodySetup;

        public TKDOPTree KDOPTree;

        public int InternalVersion;

        public bool bHaveSourceData; // UBOOL

        public FStaticMeshOptimizationSettings[] OptimizationSettings;

        public bool bHasBeenSimplified; // UBOOL

        public FStaticMeshRenderData[] LODModels;

        public FStaticMeshLODInfo[] LODInfo;

        public FRotator ThumbnailAngle;

        public float ThumbnailDistance;

        public string HighResSourceMeshName;

        public int HighResSourceMeshCRC;

        public Guid LightingGuid;

        public int VertexPositionVersionNumber;

        public float[] CachedStreamingTextureFactors;

        public bool bRemoveDegenerates; // UBOOL

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Object(out Bounds);
            stream.Int32(out BodySetup);
            stream.Object(out KDOPTree);
            stream.Int32(out InternalVersion);
            stream.BoolAsInt32(out bHaveSourceData);

#if DEBUG
            if (bHaveSourceData)
            {
                Debugger.Break();
            }
#endif

            stream.Array(out OptimizationSettings);
            stream.BoolAsInt32(out bHasBeenSimplified);
            stream.Array(out LODModels);
            stream.Array(out LODInfo);
            stream.Object(out ThumbnailAngle);
            stream.Float32(out ThumbnailDistance);
            stream.String(out HighResSourceMeshName);
            stream.Int32(out HighResSourceMeshCRC);
            stream.Guid(out LightingGuid);
            stream.Int32(out VertexPositionVersionNumber);
            stream.Float32Array(out CachedStreamingTextureFactors);
            stream.BoolAsInt32(out bRemoveDegenerates);
        }
    }
}
