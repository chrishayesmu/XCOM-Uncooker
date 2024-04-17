using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Materials
{
    public class UMaterial(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public FMaterial MaterialResource_MSP_SM3 = new FMaterial();

        public byte[] UnknownData;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            MaterialResource_MSP_SM3.Serialize(stream);

            // There's some data at the end that we don't understand; just store it opaquely for now
            long extraBytes = ExportTableEntry.SerialOffset + ExportTableEntry.SerialSize - stream.Position;
            stream.Bytes(ref UnknownData, (int)extraBytes);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UMaterial) sourceObj;

            MaterialResource_MSP_SM3.CloneFromOtherArchive(other.MaterialResource_MSP_SM3, other.Archive, Archive);
            UnknownData = other.UnknownData;
        }
    }
}
