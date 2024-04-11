using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Physics;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Models
{
    public struct FFragmentInfo : IUnrealSerializable
    {
        #region Serialized data

        public FVector Center;

        public FKConvexElem ConvexHull;

        public FBoxSphereBounds Bounds;

        public int[] Neighbours; // supposedly this is byte[], but analysis suggests it should be int[]

        public bool bCanBeDestroyed; // UBOOL

        public bool bRootFragment; // UBOOL

        public bool bNeverSpawnPhysicsChunk; // UBOOL

        public FVector AverageExteriorNormal;

        public float[] NeighbourDims;

        public int UnknownValue;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(out Center);
            stream.Object(out ConvexHull);
            stream.Object(out Bounds);
            stream.Int32Array(out Neighbours);
            stream.BoolAsInt32(out bCanBeDestroyed);
            stream.BoolAsInt32(out bRootFragment);
            stream.BoolAsInt32(out bNeverSpawnPhysicsChunk);
            stream.Object(out AverageExteriorNormal);
            stream.Float32Array(out NeighbourDims);
            stream.Int32(out UnknownValue);
        }
    }

    public struct FKConvexElem : IUnrealSerializable
    {
        #region Serialized data

        public FVector[] VertexData;

        public FPlane[] PermutedVertexData;

        public int[] FaceTriData;

        public FVector[] EdgeDirections;

        public FVector[] FaceNormalDirections;

        public FPlane[] FacePlaneData;

        public FBox ElemBox;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Array(out VertexData);
            stream.Array(out PermutedVertexData);
            stream.Int32Array(out FaceTriData);
            stream.Array(out EdgeDirections);
            stream.Array(out FaceNormalDirections);
            stream.Array(out FacePlaneData);
            stream.Object(out ElemBox);
        }
    }

    public class UFracturedStaticMesh(FArchive archive, FObjectTableEntry tableEntry) : UStaticMesh(archive, tableEntry)
    {
        #region Serialized data

        [Index(typeof(UStaticMesh))]
        public int SourceStaticMesh;

        public FFragmentInfo[] Fragments;

        public int CoreFragmentIndex;

        public int InteriorElementIndex;

        public FVector CoreMeshScale3D;

        public FVector CoreMeshOffset;

        public FRotator CoreMeshRotation;

        public FVector PlaneBias;

        public short NonCriticalBuildVersion;

        public short LicenseeNonCriticalBuildVersion;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(out SourceStaticMesh);
            stream.Array(out Fragments);
            stream.Int32(out CoreFragmentIndex);
            stream.Int32(out InteriorElementIndex);
            stream.Object(out CoreMeshScale3D);
            stream.Object(out CoreMeshOffset);
            stream.Object(out CoreMeshRotation);
            stream.Object(out PlaneBias);
            stream.Int16(out NonCriticalBuildVersion);
            stream.Int16(out LicenseeNonCriticalBuildVersion);
        }
    }
}
