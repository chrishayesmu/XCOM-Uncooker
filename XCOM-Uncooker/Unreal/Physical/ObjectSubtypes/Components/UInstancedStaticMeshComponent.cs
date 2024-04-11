using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Components
{
    public class UInstancedStaticMeshComponent(FArchive archive, FObjectTableEntry tableEntry) : UStaticMeshComponent(archive, tableEntry)
    {
        #region Serialized data

        public byte[] PerInstanceSMData;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.BulkArray(ref PerInstanceSMData);
        }
    }
}
