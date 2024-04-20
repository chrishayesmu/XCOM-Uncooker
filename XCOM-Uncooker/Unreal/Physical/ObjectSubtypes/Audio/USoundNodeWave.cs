using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Audio
{
    public class USoundNodeWave(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public FUntypedBulkData RawData;

        public FUntypedBulkData CompressedPCData;

        public FUntypedBulkData CompressedXbox360Data;

        public FUntypedBulkData CompressedPS3Data;

        public FUntypedBulkData CompressedWiiUData;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Object(ref RawData);
            stream.Object(ref CompressedPCData);
            stream.Object(ref CompressedXbox360Data);
            stream.Object(ref CompressedPS3Data);
            stream.Object(ref CompressedWiiUData);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (USoundNodeWave) sourceObj;

            RawData = other.RawData;
            CompressedPCData = other.CompressedPCData;
            CompressedXbox360Data = other.CompressedXbox360Data;
            CompressedPS3Data = other.CompressedPS3Data;
            CompressedWiiUData = other.CompressedWiiUData;
        }
    }
}
