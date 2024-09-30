using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.Components
{
    public class UDominantSpotLightComponent(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        public short[] DominantLightShadowMap;

        public override void Serialize(IUnrealDataStream stream)
        {
            if (!IsClassDefaultObject())
            {
                stream.Int16Array(ref DominantLightShadowMap);
            }

            base.Serialize(stream);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UDominantSpotLightComponent other = (UDominantSpotLightComponent)sourceObj;

            DominantLightShadowMap = other.DominantLightShadowMap;
        }
    }
}
