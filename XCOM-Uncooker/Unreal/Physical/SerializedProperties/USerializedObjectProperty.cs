using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;

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

        public override void CloneFromOtherArchive(USerializedProperty sourceProp)
        {
            base.CloneFromOtherArchive(sourceProp);

            USerializedObjectProperty other = (USerializedObjectProperty) sourceProp;

            ObjectIndex = Archive.MapIndexFromSourceArchive(other.ObjectIndex, other.Archive);
        }
    }
}
