using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical
{
    public class UConst(FArchive archive, FObjectTableEntry tableEntry) : UField(archive, tableEntry)
    {
        public string Value;

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.String(ref Value);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UConst other = (UConst) sourceObj;

            Value = other.Value;
        }
    }
}
