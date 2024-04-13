using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;

namespace XCOM_Uncooker.Unreal.Physical.SerializedProperties.ImmutableWhenCooked
{
    public class USerializedAimTransformProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "StructProperty";

        // 28 bytes of binary data; 32 bytes for each of the two property tags we apply; 8 bytes to add NAME_None
        // to the end of the property block
        public const int TaggedPropertiesSize = 28 + 32 * 2 + 8;

        #region Serialized data

        // Quat Quaternion
        // Vector Translation
        public byte[] BinaryData;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            if (stream.IsRead)
            {
                stream.Bytes(ref BinaryData, 28);
            }
            else
            {
                // To write this data back out, we need to transform it into a tagged format

                FPropertyTag tag = new FPropertyTag()
                {
                    Name = Archive.GetOrCreateName("Quaternion"),
                    Type = Archive.GetOrCreateName("StructProperty"),
                    ArrayIndex = 0,
                    Size = 16,
                    StructName = Archive.GetOrCreateName("Quat")
                };

                stream.Object(ref tag);
                stream.Bytes(ref BinaryData, tag.Size);

                tag.Name = Archive.GetOrCreateName("Translation");
                tag.StructName = Archive.GetOrCreateName("Vector");
                tag.Size = 12;

                stream.Object(ref tag);
                stream.Bytes(ref BinaryData, tag.Size, 16);

                // Denote the end of the property block
                FName nameNone = Archive.GetOrCreateName("None");
                stream.Name(ref nameNone);
            }
        }

        public override void CloneFromOtherArchive(USerializedProperty sourceProp)
        {
            USerializedAimTransformProperty other = (USerializedAimTransformProperty) sourceProp;

            BinaryData = other.BinaryData;
        }
    }
}
