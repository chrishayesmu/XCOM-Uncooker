using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Materials
{
    public class FStaticParameterSet : IUnrealSerializable
    {
        public Guid BaseMaterialId;

        public FStaticSwitchParameter[] StaticSwitchParameters;

        public FStaticComponentMaskParameter[] StaticComponentMaskParameters;
        
        public FNormalParameter[] NormalParameters;

        public FStaticTerrainLayerWeightParameter[] TerrainLayerWeightParameters;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Guid(out BaseMaterialId);
            stream.Array(out StaticSwitchParameters);
            stream.Array(out StaticComponentMaskParameters);
            stream.Array(out NormalParameters);
            stream.Array(out TerrainLayerWeightParameters);
        }
    }
}
