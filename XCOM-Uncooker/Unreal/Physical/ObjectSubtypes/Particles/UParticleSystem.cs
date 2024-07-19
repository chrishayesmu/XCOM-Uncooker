using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Particles
{
    public class UParticleSystem(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        private static readonly Logger Log = new Logger(nameof(UParticleSystem));

        public override void PostArchiveCloneComplete()
        {
            base.PostArchiveCloneComplete();

            var emittersProp = GetSerializedProperty("Emitters") as USerializedArrayProperty;
            var emittersToKeep = new List<USerializedObjectProperty>();

            // Find any emitters which were cooked out and remove them. This prevents a crash where an uncooked
            // archive is loaded in the UDK, then saved, then loaded again. For some reason this will crash if the
            // original archive had cooked-out emitters in it.
            //
            // Ideally, we'd take the cooked-out emitters out of the archive completely, but there's no good system
            // to do that right now and it doesn't seem like it's affecting anything. If the uncooked archive is loaded
            // and re-saved, the UDK appears to be removing the unused emitters anyway.
            for (int i = emittersProp.NumElements - 1; i >= 0; i--)
            {
                var emitterRef = emittersProp.Data[i] as USerializedObjectProperty;

                if (emitterRef.ObjectIndex == 0)
                {
                    continue;
                }

                var emitterObj = Archive.GetObjectByIndex(emitterRef.ObjectIndex);
                var cookedOutProp = emitterObj.GetSerializedProperty("bCookedOut") as USerializedBoolProperty;

                if (cookedOutProp == null || !cookedOutProp.BoolValue)
                {
                    emittersToKeep.Add(emitterRef);
                }
            }

            emittersProp.Data = emittersToKeep.ToArray();
            emittersProp.NumElements = emittersProp.Data.Length;
        }
    }
}
