using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;

namespace XCOM_Uncooker.Unreal.Physical.SerializedProperties
{
    public class USerializedDelegateProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "DelegateProperty";

        #region Serialized data

        public FScriptDelegate Value = new FScriptDelegate(prop.Archive);

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            Value.Serialize(stream);
        }

        public override void CloneFromOtherArchive(USerializedProperty sourceProp)
        {
            base.CloneFromOtherArchive(sourceProp);

            USerializedDelegateProperty other = (USerializedDelegateProperty) sourceProp;

            Value.CloneFromOtherArchive(other.Value);
        }
    }
}
