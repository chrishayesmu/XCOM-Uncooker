using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.Intrinsic.Core
{
    public class UMetaData(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        // Key is an object index, but we aren't actually using this field; see Serialize comments
        [Index(typeof(UObject))]
        public IDictionary<int, IDictionary<FName, string>> ObjectMetaDataMap;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            // There's only one MetaData object in all of the XCOM UPKs, in Engine.upk; that object contains just one entry,
            // which isn't very useful, so we're just going to drop the data and ignore it completely
            if (stream.IsRead)
            {
                int bytesRemaining = (int) Math.Max(0, ExportTableEntry.SerialEndPosition - stream.Position);
                stream.SkipBytes(bytesRemaining);
            }
        }
    }
}
