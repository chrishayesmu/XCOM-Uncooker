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
            stream.Object(ref LightMap);
            stream.Int32(ref Component);
            stream.Int32(ref Material);
            stream.Int16Array(ref Nodes);
            stream.Int32Array(ref ShadowMaps);
            stream.GuidArray(ref IrrelevantLights);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FModelElement) sourceObj;

            LightMap.CloneFromOtherArchive(other.LightMap, sourceArchive, destArchive);
            Component = destArchive.MapIndexFromSourceArchive(other.Component, sourceArchive);
            Material = destArchive.MapIndexFromSourceArchive(other.Material, sourceArchive);
            Nodes = other.Nodes;
            ShadowMaps = destArchive.MapIndicesFromSourceArchive(other.ShadowMaps, sourceArchive);
            IrrelevantLights = other.IrrelevantLights;
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

            stream.Int32(ref Model);
            stream.Int32(ref ZoneIndex);
            stream.Array(ref Elements);
            stream.Int16(ref ComponentIndex);
            stream.Int16Array(ref Nodes);
        }
    }
}
