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
            stream.Guid(ref BaseMaterialId);
            stream.Array(ref StaticSwitchParameters);
            stream.Array(ref StaticComponentMaskParameters);
            stream.Array(ref NormalParameters);
            stream.Array(ref TerrainLayerWeightParameters);
        }
    }
}
