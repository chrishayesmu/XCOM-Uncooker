using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties
{
    public class UByteProperty(FArchive archive, FObjectTableEntry tableEntry) : UProperty(archive, tableEntry)
    {
        public override bool IsSimpleCopyable => false;

        #region Serialized data

        /// <summary>
        /// UByteProperty is used for both byte and enum types. If this property holds an enum type,
        /// then the index of the referenced enum type will be in <c>EnumIndex</c>.
        /// </summary>
        [Index(typeof(UEnum))]
        public int EnumIndex;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(out EnumIndex);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UByteProperty other = (UByteProperty)sourceObj;

            EnumIndex = Archive.MapIndexFromSourceArchive(other.EnumIndex, other.Archive);
        }

        public override USerializedProperty CreateSerializedProperty(FArchive archive, FPropertyTag? tag)
        {
            return new USerializedByteProperty(archive, this, tag);
        }
    }
}
