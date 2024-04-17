using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.Intrinsic.Core
{
    public class UObjectRedirector(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        [Index(typeof(UObject))]
        public int DestinationObject;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(ref DestinationObject);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UObjectRedirector) sourceObj;

            DestinationObject = Archive.MapIndexFromSourceArchive(other.DestinationObject, other.Archive);
        }
    }
}
