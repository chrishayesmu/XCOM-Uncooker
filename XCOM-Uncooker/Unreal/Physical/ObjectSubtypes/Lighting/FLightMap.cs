using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Components;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Lighting
{
    public enum LightMapType
    {
        None,
        LMT_1D,
        LMT_2D
    }

    public class FLightMapData_1D : IUnrealSerializable
    {
        #region Serialized data

        public Guid[] LightGuids;

        [Index(typeof(UObject))]
        public int Owner;

        public FUntypedBulkData DirectionalSamples = new FUntypedBulkData();

        public FVector[] ScaleVectors = new FVector[3]; // fixed size array

        public FUntypedBulkData SimpleSamples = new FUntypedBulkData();

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.GuidArray(ref LightGuids);
            stream.Int32(ref Owner);
            DirectionalSamples.Serialize(stream);

            for (int i = 0; i < 3; i++)
            {
                stream.Object(ref ScaleVectors[i]);
            }

            SimpleSamples.Serialize(stream);
        }
    }

    public class FLightMapData_2D : IUnrealSerializable
    {
        #region Serialized data

        public Guid[] LightGuids;

        [Index(typeof(UObject))]
        public int[] Textures = new int[3]; // fixed size array

        public FVector[] ScaleVectors = new FVector[3]; // fixed size array

        public FVector2D CoordinateScale;

        public FVector2D CoordinateBias;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.GuidArray(ref LightGuids);

            for (int i = 0; i < 3; i++)
            {
                stream.Int32(ref Textures[i]);
                stream.Object(ref ScaleVectors[i]);
            }

            stream.Object(ref CoordinateScale);
            stream.Object(ref CoordinateBias);
        }
    }

    public class FLightMap : IUnrealSerializable
    {
        #region Serialized data

        public LightMapType Type;

        public IUnrealSerializable Data;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Enum32(ref Type);

            if (stream.IsRead)
            {
                switch (Type)
                {
                    case LightMapType.LMT_1D:
                        Data = new FLightMapData_1D();
                        break;
                    case LightMapType.LMT_2D:
                        Data = new FLightMapData_2D();
                        break;
                }
            }

            Data?.Serialize(stream);
        }
    }
}
