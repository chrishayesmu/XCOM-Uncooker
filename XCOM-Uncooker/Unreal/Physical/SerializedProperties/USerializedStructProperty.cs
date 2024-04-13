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

        public override void CloneFromOtherArchive(USerializedProperty sourceProp)
        {
            base.CloneFromOtherArchive(sourceProp);

            USerializedStructProperty other = (USerializedStructProperty) sourceProp;

            FPropertyTag? tag;

            for (int i = 0; i < other.TaggedProperties.Count; i++)
            {
                tag = null;

                if (other.TaggedProperties[i].Tag != null)
                {
                    tag = new FPropertyTag(Archive);
                    tag.Value.CloneFromOtherArchive(other.TaggedProperties[i].Tag.Value, sourceProp.Archive, Archive);
                }
                
            }
        }
    }
}
