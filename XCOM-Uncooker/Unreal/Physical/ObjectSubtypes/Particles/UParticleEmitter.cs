using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Particles
{
    public class UParticleEmitter(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            // Remove this property if it's present, although it's not entirely clear if it presents an issue
            var prop = GetSerializedProperty("bCookedOut");

            if (prop != null)
            {
                SerializedProperties.Remove(prop);
            }
        }
    }
}
