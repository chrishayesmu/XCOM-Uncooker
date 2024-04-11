using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical
{
    public class UField(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        public UField NextField => (UField) Archive.GetObjectByIndex(Next);

        #region Serialized data

        [Index(typeof(UField))]
        public int Next;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(ref Next);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UField other = (UField) sourceObj;

            Next = Archive.MapIndexFromSourceArchive(other.Next, other.Archive);
        }
    }
}
