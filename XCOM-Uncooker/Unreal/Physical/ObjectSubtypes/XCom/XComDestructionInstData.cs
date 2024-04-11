using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.XCom
{
    public struct DebrisMeshInfo : IUnrealSerializable
    {
        #region Serialized data

        public int ColumnIdx;

        [Index(typeof(UObject))]
        public int MeshComponent;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref ColumnIdx); 
            stream.Int32(ref MeshComponent);
        }
    }

public class XComDestructionInstData(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        // Key is index of a XComDecoFracLevelActor; value is index of a XComFracDecoComponent
        [Index(typeof(UObject))]
        public IDictionary<int, IList<int>> DecoFracToDecoComponents;

        // Key is index of a XComDecoFracLevelActor; value is index of a XComFracDebrisComponent
        [Index(typeof(UObject))]
        public IDictionary<int, IList<int>> DecoFracToDebrisComponents;

        // Key is index of a XComDecoFracLevelActor
        [Index(typeof(UObject))]
        public IDictionary<int, IList<DebrisMeshInfo>> DecoFracToDebrisStaticMeshInfos;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.MultiMap(ref DecoFracToDecoComponents);
            stream.MultiMap(ref DecoFracToDebrisComponents);
            stream.MultiMap(ref DecoFracToDebrisStaticMeshInfos);
        }
    }
}
