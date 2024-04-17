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
    public class USerializedArrayProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "ArrayProperty";

        #region Serialized data
        
        public int NumElements;

        public USerializedProperty[] Data;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            UProperty innerProp = (BackingProperty as UArrayProperty).Inner;
            
            stream.Int32(ref NumElements);

            if (stream.IsRead)
            {
                Data = new USerializedProperty[NumElements];
            }

            for (int i = 0; i < NumElements; i++)
            {
                if (stream.IsRead)
                {
                    Data[i] = innerProp.CreateSerializedProperty(Archive, null);
                }

                Data[i].Serialize(stream);
            }
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedArrayProperty(destArchive, BackingProperty, tag);

            other.NumElements = NumElements;

            other.Data = new USerializedProperty[Data.Length];

            for (int i = 0; i < Data.Length; i++)
            {
                other.Data[i] = Data[i].CloneToOtherArchive(destArchive);
            }

            return other;
        }
    }
}
