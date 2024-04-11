using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;

namespace XCOM_Uncooker.Unreal.Physical.SerializedProperties
{
    public class USerializedBoolProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "BoolProperty";

        public bool BoolValue => Tag != null ? Tag.Value.BoolVal : Value;

        #region Serialized data

        public bool Value;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            if (Tag != null)
            {
                // Nothing to do; tagged bool properties have all their data in the property tag
            }
            else
            {
                // If there's not a tag (e.g. we're inside an array), we need to serialize the value ourselves
                stream.Bool(ref Value);
            }
        }

        public override void CloneFromOtherArchive(USerializedProperty sourceProp)
        {
            base.CloneFromOtherArchive(sourceProp);

            USerializedBoolProperty other = (USerializedBoolProperty) sourceProp;

            Value = other.Value;
        }
    }
}
