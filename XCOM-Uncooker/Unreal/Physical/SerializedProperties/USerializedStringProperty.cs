using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;

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

        public override void CloneFromOtherArchive(USerializedProperty sourceProp)
        {
            USerializedStringProperty other = (USerializedStringProperty) sourceProp;

            Value = other.Value;
        }
    }
}
