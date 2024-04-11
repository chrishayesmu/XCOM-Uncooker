using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;

namespace XCOM_Uncooker.Unreal.Physical.SerializedProperties
{
    /// <summary>
    /// Specialized property which is similar to <see cref="USerializedArrayProperty"/>, but used for instances where we know in advance
    /// that the contents of the array are object indices. Usually, the more generic <see cref="USerializedArrayProperty"/> should be preferred,
    /// but if the class object isn't available (e.g. for <see cref="XVisGroupActor"/>), this class is necessary.
    /// </summary>
    public class USerializedIndexArrayProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "ArrayProperty";

        #region Serialized data

        public int NumElements;

        public int[] Data;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref NumElements);

            if (stream.IsRead)
            {
                Data = new int[NumElements];
            }

            for (int i = 0; i < NumElements; i++)
            {
                stream.Int32(ref Data[i]);
            }
        }

        public override void CloneFromOtherArchive(USerializedProperty sourceProp)
        {
            base.CloneFromOtherArchive(sourceProp);

            USerializedIndexArrayProperty other = (USerializedIndexArrayProperty) sourceProp;

            NumElements = other.NumElements;

            Data = new int[NumElements];

            for (int i = 0; i < NumElements; i++)
            {
                Data[i] = Archive.MapIndexFromSourceArchive(other.Data[i], other.Archive);
            }
        }
    }
}
