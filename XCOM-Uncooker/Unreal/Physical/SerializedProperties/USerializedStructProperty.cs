using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;

namespace XCOM_Uncooker.Unreal.Physical.SerializedProperties
{
    public class USerializedStructProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "StructProperty";

        #region Serialized data
        
        public List<USerializedProperty> TaggedProperties = [];
        
        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            // Read the properties like any other props
            var structProp = (UStructProperty) BackingProperty;
            structProp.StructDefinition.SerializeTaggedProperties(TaggedProperties, stream);
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedStructProperty(destArchive, BackingProperty, tag);

            other.TaggedProperties = new List<USerializedProperty>(TaggedProperties.Count);

            for (int i = 0; i < TaggedProperties.Count; i++)
            {
                var clonedProp = TaggedProperties[i].CloneToOtherArchive(destArchive);
                other.TaggedProperties.Add(clonedProp);
            }

            return other;
        }
    }
}
