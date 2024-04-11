using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Components
{
    public class UDominantDirectionalLightComponent(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public short[] DominantLightShadowMap;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            if (!IsClassDefaultObject())
            {
                stream.Int16Array(out DominantLightShadowMap);
            }

            base.Serialize(stream);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UDominantDirectionalLightComponent other = (UDominantDirectionalLightComponent)sourceObj;

            DominantLightShadowMap = other.DominantLightShadowMap;
        }
    }
}
