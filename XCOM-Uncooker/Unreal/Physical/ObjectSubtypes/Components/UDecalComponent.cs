using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Actor;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Components
{
    public struct FDecalVertex : IUnrealSerializable
    {
        #region Serialized data

        public FVector Position;

        public FPackedNormal TangentX;

        public FPackedNormal TangentZ;

        public FVector2D LightMapCoordinate;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref Position);
            stream.Object(ref TangentX);
            stream.Object(ref TangentZ);
            stream.Object(ref LightMapCoordinate);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FDecalVertex) sourceObj;

            Position = other.Position;
            TangentX = other.TangentX;
            TangentZ = other.TangentZ;
            LightMapCoordinate = other.LightMapCoordinate;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }

    public struct FStaticReceiverData : IUnrealSerializable
    {
        #region Serialized data

        [Index(typeof(UObject))]
        public int Component;

        public FDecalVertex[] Vertices;

        public short[] Indices;

        public uint NumTriangles;

        [Index(typeof(UObject))]
        public int LightMap1D;

        [Index(typeof(UObject))]
        public int ShadowMap1D;

        public int Data;

        public int InstanceIndex;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref Component);
            stream.BulkArray(ref Vertices, 28);
            stream.BulkArray(ref Indices);
            stream.UInt32(ref NumTriangles);
            stream.Int32(ref LightMap1D);
            stream.Int32(ref ShadowMap1D);
            stream.Int32(ref Data);
            stream.Int32(ref InstanceIndex);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FStaticReceiverData) sourceObj;

            Component = destArchive.MapIndexFromSourceArchive(other.Component, sourceArchive);
            Vertices = other.Vertices;
            Indices = other.Indices;
            NumTriangles = other.NumTriangles;
            LightMap1D = destArchive.MapIndexFromSourceArchive(other.LightMap1D, sourceArchive);
            ShadowMap1D = destArchive.MapIndexFromSourceArchive(other.ShadowMap1D, sourceArchive);
            Data = other.Data;
            InstanceIndex = other.InstanceIndex;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
            dependencyIndices.Add(Component);
            dependencyIndices.Add(LightMap1D);
            dependencyIndices.Add(ShadowMap1D);
        }
    }

    public class UDecalComponent(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public FStaticReceiverData[] StaticReceivers;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Array(ref StaticReceivers);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UDecalComponent) sourceObj;

            StaticReceivers = IUnrealSerializable.Clone(other.StaticReceivers, other.Archive, Archive);
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            foreach (var receiver in StaticReceivers)
            {
                receiver.PopulateDependencies(dependencyIndices);
            }
        }
    }
}
