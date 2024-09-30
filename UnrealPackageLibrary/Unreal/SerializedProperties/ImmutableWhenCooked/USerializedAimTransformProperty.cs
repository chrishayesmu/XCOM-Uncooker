using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.Intrinsic.Core.Properties;

namespace UnrealArchiveLibrary.Unreal.SerializedProperties.ImmutableWhenCooked
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
                FName NAME_None = Archive.GetOrCreateName("None");
                stream.Name(ref NAME_None);
            }
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedAimTransformProperty(destArchive, null, tag);

            other.BinaryData = BinaryData;

            // Ensure all of these names exist in the destination archive
            destArchive.GetOrCreateName("None");
            destArchive.GetOrCreateName("Quat");
            destArchive.GetOrCreateName("Quaternion");
            destArchive.GetOrCreateName("StructProperty");
            destArchive.GetOrCreateName("Translation");
            destArchive.GetOrCreateName("Vector");

            return other;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }
}
