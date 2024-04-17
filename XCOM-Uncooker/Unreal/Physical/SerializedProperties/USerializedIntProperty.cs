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
    public class USerializedIntProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "IntProperty";

        #region Serialized data

        public int Value;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref Value);
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedIntProperty(destArchive, null, tag);

            other.Value = Value;

            return other;
        }
    }
}
