using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

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
            stream.Object(out Position);
            stream.Object(out TangentX);
            stream.Object(out TangentZ);
            stream.Object(out LightMapCoordinate);
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
            stream.Int32(out Component);
            stream.BulkArray(out Vertices, 28);
            stream.BulkArray(out Indices);
            stream.UInt32(out NumTriangles);
            stream.Int32(out LightMap1D);
            stream.Int32(out ShadowMap1D);
            stream.Int32(out Data);
            stream.Int32(out InstanceIndex);
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

            stream.Array(out StaticReceivers);
        }
    }
}
