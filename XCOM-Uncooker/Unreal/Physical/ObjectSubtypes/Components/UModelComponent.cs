using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Lighting;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Models;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Components
{
    public struct FModelElement : IUnrealSerializable
    {
        public FModelElement() {}

        #region Serialized data

        public FLightMap LightMap = new FLightMap();

        [Index(typeof(UModelComponent))]
        public int Component;

        [Index(typeof(UObject))]
        public int Material;

        public short[] Nodes;

        [Index(typeof(UObject))]
        public int[] ShadowMaps;

        public Guid[] IrrelevantLights;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(out LightMap);
            stream.Int32(out Component);
            stream.Int32(out Material);
            stream.Int16Array(out Nodes);
            stream.Int32Array(out ShadowMaps);
            stream.GuidArray(out IrrelevantLights);
        }
    }

    public class UModelComponent(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data
        
        [Index(typeof(UModel))]
        public int Model;

        public int ZoneIndex;

        public FModelElement[] Elements;

        public short ComponentIndex;

        public short[] Nodes;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(out Model);
            stream.Int32(out ZoneIndex);
            stream.Array(out Elements);
            stream.Int16(out ComponentIndex);
            stream.Int16Array(out Nodes);
        }
    }
}
