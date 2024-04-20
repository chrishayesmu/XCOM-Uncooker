using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties.ImmutableWhenCooked;

namespace XCOM_Uncooker.Unreal.Physical.SerializedProperties
{
    /// <summary>
    /// This property can represent either a single byte of data, or an enum value. Enum values are a single byte at
    /// runtime, but they are serialized to disk using their enum name (8 bytes) instead of their integer value. This
    /// allows for enum values to be added to or rearranged without invalidating previously-created objects.
    /// </summary>
    public class USerializedByteProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        private static readonly Logger Log = new Logger(nameof(USerializedByteProperty));

        public override string TagType => "ByteProperty";

        private bool IsEnumType => (BackingProperty as UByteProperty)?.EnumIndex != 0 || (!Tag?.EnumName.IsNone() ?? false);

        #region Serialized data

        public byte ByteValue;

        public FName EnumValue;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            UByteProperty prop = BackingProperty as UByteProperty;
            bool isEnumType = prop?.EnumIndex != 0 || (!Tag?.EnumName.IsNone() ?? false);

            if (!isEnumType)
            {
                // With no enum name set, this is an actual byte variable, and will be serialized as such
                stream.UInt8(ref ByteValue);
            }
            else
            {
                stream.Name(ref EnumValue);
            }
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedByteProperty(destArchive, BackingProperty, tag);

            other.ByteValue = ByteValue;
            other.EnumValue = destArchive.MapNameFromSourceArchive(EnumValue);

            return other;
        }

    }
}
