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
    public class USerializedObjectProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "ObjectProperty";

        #region Serialized data

        [Index(typeof(UObject))]
        public int ObjectIndex;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref ObjectIndex);
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedObjectProperty(destArchive, BackingProperty, tag);

            other.ObjectIndex = destArchive.MapIndexFromSourceArchive(ObjectIndex, Archive);

            return other;
        }
    }
}
