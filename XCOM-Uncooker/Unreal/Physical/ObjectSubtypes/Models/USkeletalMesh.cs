using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Physics;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Models
{
    [FixedSize(96)]
    public struct TkDOP : IUnrealSerializable
    {
        public TkDOP() {}

        #region Serialized data

        public float[] Min = new float[3]; // fixed size array

        public float[] Max = new float[3]; // fixed size array

        // public FPlane[] Min = new FPlane[3]; // fixed size array
        // 
        // public FPlane[] Max = new FPlane[3]; // fixed size array

        #endregion
     
        public void Serialize(IUnrealDataStream stream)
        {
            for (int i = 0; i < 3; i++)
            {
                stream.Float32(ref Min[i]);
            }

            for (int i = 0; i < 3; i++)
            {
                stream.Float32(ref Max[i]);
            }
        }
    }

    public struct TKDOPTree : IUnrealSerializable
    {
        #region Serialized data

        // This struct serializes 3 things: its bounds, its nodes, and its triangles. The nodes and triangles are
        // bulk-serialized, and don't contain any archive-specific data, so we don't have to understand their format
        // at all.

        public TkDOP RootBound;

        public byte[] Nodes;

        public byte[] Triangles;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref RootBound);
            stream.BulkArray(ref Nodes);
            stream.BulkArray(ref Triangles);
        }
    }

    public struct VJointPos : IUnrealSerializable
    {
        #region Serialized data

        public FQuat Orientation;
        public FVector Position;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref Orientation);
            stream.Object(ref Position);
        }
    }

    public struct FMeshBone : IUnrealSerializable
    {
        #region Serialized data

        public FName Name;
        public uint Flags;
        public VJointPos BonePos;
        public int NumChildren;
        public int ParentIndex;
        public FColor BoneColor;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Name(ref Name);
            stream.UInt32(ref Flags);
            stream.Object(ref BonePos);
            stream.Int32(ref NumChildren);
            stream.Int32(ref ParentIndex);
            stream.Object(ref BoneColor);
        }
    }

    public struct FMultiSizeIndexContainer : IUnrealSerializable
    {
        #region Serialized data

        public bool NeedsCPUAccess; // UBOOL

        public byte DataTypeSize;

        public byte[] IndexBuffer;

        #endregion
     
        public void Serialize(IUnrealDataStream stream)
        {
            stream.BoolAsInt32(ref NeedsCPUAccess);
            stream.UInt8(ref DataTypeSize);
            stream.BulkArray(ref IndexBuffer);
        }
    }

    public struct FPerPolyBoneCollisionData : IUnrealSerializable
    {
        #region Serialized data

        public TKDOPTree KDOPTree;

        public FVector[] CollisionVerts;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref KDOPTree);
            stream.Array(ref CollisionVerts);
        }
    }

    public struct FRigidSkinVertex : IUnrealSerializable
    {
        public FRigidSkinVertex() {}

        #region Serialized data

        public FVector Position;
        
        public FPackedNormal TangentX;
        public FPackedNormal TangentY;
        public FPackedNormal TangentZ;

        public FVector2D[] UVs = new FVector2D[4]; // fixed size array

        public FColor Color;

        public byte Bone;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref Position);
            stream.Object(ref TangentX);
            stream.Object(ref TangentY);
            stream.Object(ref TangentZ);

            for (int i = 0; i < 4; i++)
            {
                stream.Object(ref UVs[i]);
            }

            stream.Object(ref Color);
            stream.UInt8(ref Bone);
        }
    }

    public struct FSoftSkinVertex : IUnrealSerializable
    {
        public FSoftSkinVertex() {}

        #region Serialized data

        public FVector Position;

        public FPackedNormal TangentX;
        public FPackedNormal TangentY;
        public FPackedNormal TangentZ;

        public FVector2D[] UVs = new FVector2D[4]; // fixed size array

        public FColor Color;

        public byte[] InfluenceBones;
        public byte[] InfluenceWeights;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref Position);
            stream.Object(ref TangentX);
            stream.Object(ref TangentY);
            stream.Object(ref TangentZ);

            for (int i = 0; i < 4; i++)
            {
                stream.Object(ref UVs[i]);
            }

            stream.Bytes(ref InfluenceBones, 4);
            stream.Bytes(ref InfluenceWeights, 4);
        }
    }

    public struct FSkelMeshChunk : IUnrealSerializable
    {
        #region Serialized data

        public uint BaseVertexIndex;

        public FRigidSkinVertex[] RigidVertices;

        public FSoftSkinVertex[] SoftVertices;

        public short[] BoneMap;

        public int NumRigidVertices;

        public int NumSoftVertices;

        public int MaxBoneInfluences;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.UInt32(ref BaseVertexIndex);
            stream.Array(ref RigidVertices);
            stream.Array(ref SoftVertices);
            stream.Int16Array(ref BoneMap);
            stream.Int32(ref NumRigidVertices);
            stream.Int32(ref NumSoftVertices);
            stream.Int32(ref MaxBoneInfluences);
        }
    }

    public struct FSkelMeshSection : IUnrealSerializable
    {
        #region Serialized data

        public short MaterialIndex;
        public short ChunkIndex;
        public int BaseIndex;
        public int NumTriangles;
        public byte TriangleSorting;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int16(ref MaterialIndex);
            stream.Int16(ref ChunkIndex);
            stream.Int32(ref BaseIndex);
            stream.Int32(ref NumTriangles);
            stream.UInt8(ref TriangleSorting);
        }
    }

    public struct FSkeletalMeshSourceData : IUnrealSerializable
    {
        #region Serialized data

        public bool bHaveSourceData; // UBOOL

        public FStaticLODModel LODModel;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.BoolAsInt32(ref bHaveSourceData);

            if (bHaveSourceData)
            {
                Debugger.Break();

                stream.Object(ref LODModel);
            }
        }
    }

    public struct FSkeletalMeshVertexBuffer : IUnrealSerializable
    {
        #region Serialized data

        public uint NumTexCoords;

        public bool bUseFullPrecisionUVs; // UBOOL

        public bool bUsePackedPosition; // UBOOL

        public FVector MeshExtension;

        public FVector MeshOrigin;

        // We just treat this field as binary data, since it's bulk serialized
        // public TSkeletalMeshVertexData<FGPUSkinVertexBase> VertexData;
        public byte[] VertexData;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.UInt32(ref NumTexCoords);
            stream.BoolAsInt32(ref bUseFullPrecisionUVs);
            stream.BoolAsInt32(ref bUsePackedPosition);
            stream.Object(ref MeshExtension);
            stream.Object(ref MeshOrigin);
            stream.BulkArray(ref VertexData);
        }

        private void CreateVertexDataBuffer()
        {
            // This function is currently unused; see FGPUSkinVertexBase for why

            if (bUseFullPrecisionUVs)
            {
                if (bUsePackedPosition)
                {
                    // VertexData = new TGPUSkinVertexFloat32Uvs32Xyz();
                }
                else
                {
                    // VertexData = new TGPUSkinVertexFloat32Uvs();
                }
            }
            else
            {
                if (bUsePackedPosition)
                {
                    // VertexData = new TGPUSkinVertexFloat16Uvs32Xyz();
                }
                else
                {
                    // VertexData = new TGPUSkinVertexFloat16Uvs();
                }
            }
        }
    }

    // The real version of this struct is more complicated and has more layers; this version
    // is just pared down to what we care abref for XCOM EW
    public struct FSkeletalMeshVertexColorBuffer : IUnrealSerializable
    {
        #region Serialized data

        public FColor[] Data;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.BulkArray(ref Data, 4);
        }
    }

    public struct FSkeletalMeshVertexInfluences : IUnrealSerializable
    {
        #region Serialized data

        public FVertexInfluence[] Influences;

        // Key here is actually two 32-bit integers in a struct but we don't care
        public IDictionary<long, int[]> VertexInfluenceMapping;

        public FSkelMeshSection[] Sections;

        public FSkelMeshChunk[] Chunks;

        public byte[] RequiredBones;

        public byte Usage;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Array(ref Influences);
            stream.Map(ref VertexInfluenceMapping);
            stream.Array(ref Sections);
            stream.Array(ref Chunks);
            stream.ByteArray(ref RequiredBones);
            stream.UInt8(ref Usage);
        }
    }

    public struct FStaticLODModel : IUnrealSerializable
    {
        private UObject _Owner;
        public UObject Owner
        {
            get => _Owner;
            set { _Owner = value; }
        }

        #region Serialized data

        public FSkelMeshSection[] Sections;

        public FMultiSizeIndexContainer MultiSizeIndexContainer;

        public short[] ActiveBoneIndices;

        public FSkelMeshChunk[] Chunks;

        public uint Size;

        public uint NumVertices;

        public byte[] RequiredBones;

        // Technically this is FIntBulkData, but since we're never interpreting it, we don't care
        public FUntypedBulkData RawPointIndices;

        public uint NumTexCoords;

        public FSkeletalMeshVertexBuffer VertexBufferGPUSkin;

        public FSkeletalMeshVertexColorBuffer ColorVertexBuffer;

        public FSkeletalMeshVertexInfluences[] VertexInfluences;

        public FMultiSizeIndexContainer AdjacencyMultiSizeIndexContainer;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Array(ref Sections);
            stream.Object(ref MultiSizeIndexContainer);
            stream.Int16Array(ref ActiveBoneIndices);
            stream.Array(ref Chunks);
            stream.UInt32(ref Size);
            stream.UInt32(ref NumVertices);
            stream.ByteArray(ref RequiredBones);
            stream.Object(ref RawPointIndices);
            stream.UInt32(ref NumTexCoords);
            stream.Object(ref VertexBufferGPUSkin);

            var mesh = (USkeletalMesh) Owner;

#if DEBUG
            if (mesh == null)
            {
                throw new Exception($"{nameof(FStaticLODModel)} has a null owner!");
            }
#endif

            var bHasVertexColorsProp = mesh.GetSerializedProperty("bHasVertexColors") as USerializedBoolProperty;
            bool bHasVertexColors = bHasVertexColorsProp?.BoolValue ?? false;

            // ColorVertexBuffer is only serialized when the owning SkeletalMesh's bHasVertexColors is true
            if (bHasVertexColors)
            {
                stream.Object(ref ColorVertexBuffer);
            }

            stream.Array(ref VertexInfluences);
            stream.Object(ref AdjacencyMultiSizeIndexContainer);
        }
    }

    public struct FVertexInfluence : IUnrealSerializable
    {
        #region Serialized data

        public uint Weights;
        public uint Bones;

        #endregion
     
        public void Serialize(IUnrealDataStream stream)
        {
            stream.UInt32(ref Weights);
            stream.UInt32(ref Bones);
        }
    }

    public class USkeletalMesh(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public FBoxSphereBounds Bounds;

        [Index(typeof(UObject))]
        public int[] Materials;

        public FVector Origin;

        public FRotator RotOrigin;

        public FMeshBone[] RefSkeleton;

        public int SkeletalDepth;

        public FStaticLODModel[] LODModels;

        public IDictionary<FName, int> NameIndexMap;

        public FPerPolyBoneCollisionData[] PerPolyBoneKDOPs;

        public string[] BoneBreakNames;

        public byte[] BoneBreakOptions;

        [Index(typeof(UObject))]
        public int[] ClothingAssets;

        public float[] CachedStreamingTextureFactors;

        public FSkeletalMeshSourceData SkelSourceData;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Object(ref Bounds);
            stream.Int32Array(ref Materials);
            stream.Object(ref Origin);
            stream.Object(ref RotOrigin);
            stream.Array(ref RefSkeleton);
            stream.Int32(ref SkeletalDepth);
            stream.Array(ref LODModels, owner: this);
            stream.Map(ref NameIndexMap);
            stream.Array(ref PerPolyBoneKDOPs);
            stream.StringArray(ref BoneBreakNames);
            stream.ByteArray(ref BoneBreakOptions);
            stream.Int32Array(ref ClothingAssets);
            stream.Float32Array(ref CachedStreamingTextureFactors);
            stream.Object(ref SkelSourceData);
        }
    }
}
