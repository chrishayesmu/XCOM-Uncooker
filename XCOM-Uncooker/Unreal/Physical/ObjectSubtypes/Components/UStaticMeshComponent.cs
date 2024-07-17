using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Actor;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Lighting;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Components
{
    public struct FColorVertexBuffer : IUnrealSerializable
    {
        #region Serialized data

        public uint Stride;

        public uint NumVertices;

        public FByteArrayWithSize VertexData;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.UInt32(ref Stride);
            stream.UInt32(ref NumVertices);

            if (NumVertices > 0)
            {
                stream.Object(ref VertexData);
            }
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FColorVertexBuffer) sourceObj;

            Stride = other.Stride;
            NumVertices = other.NumVertices;
            VertexData = other.VertexData;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }

    public struct FStaticMeshComponentLODInfo : IUnrealSerializable
    {
        public FStaticMeshComponentLODInfo() {}

        #region Serialized data

        [Index(typeof(UObject))]
        public int[] ShadowMaps;

        [Index(typeof(UObject))]
        public int[] ShadowVertexBuffers;

        public FLightMap LightMap = new FLightMap();

        public bool bLoadVertexColorData;

        public FColorVertexBuffer OverrideVertexColors = new FColorVertexBuffer();

        public FVector[] VertexColorPositions;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32Array(ref ShadowMaps);
            stream.Int32Array(ref ShadowVertexBuffers);
            stream.Object(ref LightMap);
            stream.Bool(ref bLoadVertexColorData);

            if (bLoadVertexColorData)
            {
                stream.Object(ref OverrideVertexColors);
            }

            stream.Array(ref VertexColorPositions);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FStaticMeshComponentLODInfo) sourceObj;

            ShadowMaps = destArchive.MapIndicesFromSourceArchive(other.ShadowMaps, sourceArchive);
            ShadowVertexBuffers = destArchive.MapIndicesFromSourceArchive(other.ShadowVertexBuffers, sourceArchive);
            LightMap.CloneFromOtherArchive(other.LightMap, sourceArchive, destArchive);
            bLoadVertexColorData = other.bLoadVertexColorData;
            OverrideVertexColors.CloneFromOtherArchive(other.OverrideVertexColors, sourceArchive, destArchive);
            VertexColorPositions = IUnrealSerializable.Clone(other.VertexColorPositions, sourceArchive, destArchive);
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
            dependencyIndices.AddRange(ShadowMaps);
            dependencyIndices.AddRange(ShadowVertexBuffers);
            LightMap.PopulateDependencies(dependencyIndices);
        }
    }

    public class UStaticMeshComponent(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public FStaticMeshComponentLODInfo[] LODData;

        public FStaticMeshComponentLODInfo[] SwapMeshData = new FStaticMeshComponentLODInfo[2]; // XCOM addition; fixed size array

        [Index(typeof(UObject))]
        public int[] SwapStaticMeshes = new int[2]; // XCOM addition; fixed size array

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Array(ref LODData);

            if (stream.IsRead)
            {
                stream.Object(ref SwapMeshData[0]);
                stream.Object(ref SwapMeshData[1]);
                stream.Int32(ref SwapStaticMeshes[0]);
                stream.Int32(ref SwapStaticMeshes[1]);
            }
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UStaticMeshComponent) sourceObj;

            LODData = IUnrealSerializable.Clone(other.LODData, other.Archive, Archive);

            SwapMeshData = IUnrealSerializable.Clone(other.SwapMeshData, other.Archive, Archive);
            SwapStaticMeshes = Archive.MapIndicesFromSourceArchive(other.SwapStaticMeshes, other.Archive);
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            foreach (var lod in LODData)
            {
                lod.PopulateDependencies(dependencyIndices);
            }

            foreach (var swapMesh in SwapMeshData)
            {
                swapMesh.PopulateDependencies(dependencyIndices);
            }

            dependencyIndices.AddRange(SwapStaticMeshes);
        }
    }
}
