using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical
{
    public class UEnum(FArchive archive, FObjectTableEntry tableEntry) : UField(archive, tableEntry)
    {
        public FName[] Names;

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.NameArray(ref Names);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UEnum other = (UEnum) sourceObj;

            Names = CloneNameArrayFromArchive(other.Names);
        }
    }
}
