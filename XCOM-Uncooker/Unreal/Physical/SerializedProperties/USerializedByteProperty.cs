using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;

namespace XCOM_Uncooker.Unreal.Physical.SerializedProperties
{
    public class USerializedByteProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        private static readonly Logger Log = new Logger(nameof(USerializedByteProperty));

        public override string TagType => "ByteProperty";

        #region Serialized data

        public ulong Value;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            UByteProperty prop = BackingProperty as UByteProperty;
            bool isEnumType = prop?.EnumIndex != 0 || (!Tag?.EnumName.IsNone() ?? false);

            if (!isEnumType)
            {
                // With no enum name set, this is an actual byte variable, and will be serialized as such
                if (stream.IsRead)
                {
                    byte[] bytes = new byte[1];
                    stream.Bytes(out bytes, 1);
                    Value = bytes[0];
                }
                else
                {
                    Log.Warning($"Serialized writes not implemented yet");
                }
            }
            else
            {
                stream.UInt64(out Value);
            }
        }

        public override void CloneFromOtherArchive(USerializedProperty sourceProp)
        {
            base.CloneFromOtherArchive(sourceProp);

            USerializedByteProperty other = (USerializedByteProperty) sourceProp;

            Value = other.Value;
        }
    }
}
