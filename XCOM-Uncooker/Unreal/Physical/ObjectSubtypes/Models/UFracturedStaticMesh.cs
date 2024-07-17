using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Actor;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Physics;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Models
{
    public struct FFragmentInfo : IUnrealSerializable
    {
        public static Dictionary<int, int> UnknownValCount = new Dictionary<int, int>();

        #region Serialized data

        public FVector Center;

        public FKConvexElem ConvexHull;

        public FBoxSphereBounds Bounds;

        public int[] Neighbours; // byte[] in normal UE3, int[] in XCOM

        public bool bCanBeDestroyed; // UBOOL

        public bool bRootFragment; // UBOOL

        public bool bNeverSpawnPhysicsChunk; // UBOOL

        public FVector AverageExteriorNormal;

        public float[] NeighbourDims;

        public int UnknownValue;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref Center);
            stream.Object(ref ConvexHull);
            stream.Object(ref Bounds);

            if (stream.IsRead)
            {
                stream.Int32Array(ref Neighbours);
            }
            else
            {
                // UDK expects an array of bytes
                byte[] neighboursAsBytes = new byte[Neighbours.Length];

                for (int i = 0; i < Neighbours.Length; i++)
                {
                    neighboursAsBytes[i] = (byte) Math.Clamp(Neighbours[i], 0, 255);
                }

                stream.ByteArray(ref neighboursAsBytes);
            }

            stream.BoolAsInt32(ref bCanBeDestroyed);
            stream.BoolAsInt32(ref bRootFragment);
            stream.BoolAsInt32(ref bNeverSpawnPhysicsChunk);
            stream.Object(ref AverageExteriorNormal);
            stream.Float32Array(ref NeighbourDims);

            if (stream.IsRead)
            {
                stream.Int32(ref UnknownValue);

                if (UnknownValCount.TryGetValue(UnknownValue, out int value))
                {
                    UnknownValCount[UnknownValue] = value + 1;
                }
                else
                {
                    UnknownValCount[UnknownValue] = 1;
                }
            }
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FFragmentInfo) sourceObj;

            Center = other.Center;
            ConvexHull = other.ConvexHull;
            Bounds = other.Bounds;
            Neighbours = other.Neighbours;
            bCanBeDestroyed = other.bCanBeDestroyed;
            bRootFragment = other.bRootFragment;
            bNeverSpawnPhysicsChunk = other.bNeverSpawnPhysicsChunk;
            AverageExteriorNormal = other.AverageExteriorNormal;
            NeighbourDims = other.NeighbourDims;
            UnknownValue = other.UnknownValue;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
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
            stream.Array(ref VertexData);
            stream.Array(ref PermutedVertexData);
            stream.Int32Array(ref FaceTriData);
            stream.Array(ref EdgeDirections);
            stream.Array(ref FaceNormalDirections);
            stream.Array(ref FacePlaneData);
            stream.Object(ref ElemBox);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FKConvexElem) sourceObj;

            VertexData = other.VertexData;
            PermutedVertexData = other.PermutedVertexData;
            FaceTriData = other.FaceTriData;
            EdgeDirections = other.EdgeDirections;
            FaceNormalDirections = other.FaceNormalDirections;
            FacePlaneData = other.FacePlaneData;
            ElemBox = other.ElemBox;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
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

            stream.Int32(ref SourceStaticMesh);
            stream.Array(ref Fragments);
            stream.Int32(ref CoreFragmentIndex);
            stream.Int32(ref InteriorElementIndex);
            stream.Object(ref CoreMeshScale3D);
            stream.Object(ref CoreMeshOffset);
            stream.Object(ref CoreMeshRotation);
            stream.Object(ref PlaneBias);
            stream.Int16(ref NonCriticalBuildVersion);
            stream.Int16(ref LicenseeNonCriticalBuildVersion);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UFracturedStaticMesh) sourceObj;

            SourceStaticMesh = Archive.MapIndexFromSourceArchive(other.SourceStaticMesh, other.Archive);
            Fragments = other.Fragments;
            CoreFragmentIndex = other.CoreFragmentIndex;
            InteriorElementIndex = other.InteriorElementIndex;
            CoreMeshScale3D = other.CoreMeshScale3D;
            CoreMeshOffset = other.CoreMeshOffset;
            CoreMeshRotation = other.CoreMeshRotation;
            PlaneBias = other.PlaneBias;
            NonCriticalBuildVersion = other.NonCriticalBuildVersion;
            LicenseeNonCriticalBuildVersion = other.LicenseeNonCriticalBuildVersion;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            dependencyIndices.Add(SourceStaticMesh);
        }
    }
}
