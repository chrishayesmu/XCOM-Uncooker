using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties
{
    public class UFloatProperty(FArchive archive, FObjectTableEntry tableEntry) : UProperty(archive, tableEntry)
    {
        public override USerializedProperty CreateSerializedProperty(FArchive archive, FPropertyTag? tag)
        {
            return new USerializedFloatProperty(archive, this, tag);
        }
    }
}
