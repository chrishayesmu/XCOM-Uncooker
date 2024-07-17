using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;

namespace XCOM_Uncooker.Unreal.Physical.SerializedProperties.ImmutableWhenCooked
{
    public class USerializedInstancedStaticMeshInstanceDataProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "StructProperty";

        #region Serialized data

        // Matrix Transform (64)
        // Vector2D LightmapUVBias (8)
        // Vector2D ShadowmapUVBias (8)
        public byte[] BinaryData;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            if (stream.IsRead)
            {
                stream.Bytes(ref BinaryData, 80);
            }
            else
            {
                FPropertyTag tag = new FPropertyTag()
                {
                    Name = Archive.GetOrCreateName("Transform"),
                    Type = Archive.GetOrCreateName("StructProperty"),
                    ArrayIndex = 0,
                    Size = 64,
                    StructName = Archive.GetOrCreateName("Matrix")
                };

                stream.Object(ref tag);
                stream.Bytes(ref BinaryData, tag.Size);

                tag.Name = Archive.GetOrCreateName("LightmapUVBias");
                tag.StructName = Archive.GetOrCreateName("Vector2D");
                tag.Size = 8;

                stream.Object(ref tag);
                stream.Bytes(ref BinaryData, tag.Size, 64);

                tag.Name = Archive.GetOrCreateName("ShadowmapUVBias");

                stream.Object(ref tag);
                stream.Bytes(ref BinaryData, tag.Size, 64 + 8);
            }
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedInstancedStaticMeshInstanceDataProperty(destArchive, null, tag);

            other.BinaryData = BinaryData;

            // Ensure all of these names exist in the destination archive
            destArchive.GetOrCreateName("LightmapUVBias");
            destArchive.GetOrCreateName("Matrix");
            destArchive.GetOrCreateName("ShadowmapUVBias");
            destArchive.GetOrCreateName("StructProperty");
            destArchive.GetOrCreateName("Transform");
            destArchive.GetOrCreateName("Vector2D");

            return other;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }
}
