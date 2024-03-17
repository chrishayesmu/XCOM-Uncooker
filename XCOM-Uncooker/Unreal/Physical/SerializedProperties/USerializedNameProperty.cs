using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;

namespace XCOM_Uncooker.Unreal.Physical.SerializedProperties
{
    public class USerializedNameProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "NameProperty";

        #region Serialized data

        public FName Value;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            stream.Name(out Value);
        }

        public override void CloneFromOtherArchive(USerializedProperty sourceProp)
        {
            base.CloneFromOtherArchive(sourceProp);

            USerializedNameProperty other = (USerializedNameProperty) sourceProp;

            Value = Archive.MapNameFromSourceArchive(other.Value);
        }
    }
}
