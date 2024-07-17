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
    public class USerializedStringProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "StrProperty";

        #region Serialized data

        public string Value;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            stream.String(ref Value);
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedStringProperty(destArchive, BackingProperty, tag);

            other.Value = Value;

            return other;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }
}
