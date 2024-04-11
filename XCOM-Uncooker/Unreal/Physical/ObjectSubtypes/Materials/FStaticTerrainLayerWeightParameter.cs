using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Materials
{
    public class FStaticTerrainLayerWeightParameter : IUnrealSerializable
    {
        #region Serialized data

        public FName ParameterName;

        public int WeightmapIndex;
        
        public bool Override;

        public Guid ExpressionGuid;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Name(out ParameterName);
            stream.Int32(out WeightmapIndex);
            stream.BoolAsInt32(out Override);
            stream.Guid(out ExpressionGuid);
        }
    }
}
