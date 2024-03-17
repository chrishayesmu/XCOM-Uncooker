using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes;

namespace XCOM_Uncooker.Unreal.Physical.Intrinsic.Engine
{
    public class ULevel(FArchive archive, FObjectTableEntry tableEntry) : ULevelBase(archive, tableEntry)
    {
        [Index(typeof(UModel))]
        public int Model;
    }
}
