using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;

namespace XCOM_Uncooker.Unreal.Physical.SerializedProperties
{
    public class USerializedMapProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        private static readonly Logger Log = new Logger(nameof(USerializedMapProperty));

        public override string TagType => "MapProperty";

        public override void Serialize(IUnrealDataStream stream)
        {
            if (Tag.HasValue && Tag.Value.Size > 0)
            {
                Log.Warning($"Asked to serialize {Tag.Value.Size} bytes, but we don't know how to serialize this");
            }
        }
    }
}
