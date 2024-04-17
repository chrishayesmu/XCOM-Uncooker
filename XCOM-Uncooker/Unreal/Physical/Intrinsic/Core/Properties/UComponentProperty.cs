using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties
{
    public class UComponentProperty(FArchive archive, FObjectTableEntry tableEntry) : UObjectProperty(archive, tableEntry)
    {
        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);
        }
    }
}
