using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;

namespace XCOM_Uncooker.Unreal.Physical.SerializedProperties
{
    public class USerializedFloatProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "FloatProperty";

        #region Serialized data

        public float Value;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            stream.Float32(ref Value);
        }

        public override void CloneFromOtherArchive(USerializedProperty sourceProp)
        {
            base.CloneFromOtherArchive(sourceProp);

            USerializedFloatProperty other = (USerializedFloatProperty) sourceProp;

            Value = other.Value;
        }
    }
}
