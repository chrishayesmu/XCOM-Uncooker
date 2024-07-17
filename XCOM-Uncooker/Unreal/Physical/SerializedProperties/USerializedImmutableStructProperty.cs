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
    /// <summary>
    /// Represents an opaque container of data from an immutable struct. Since immutable structs use binary serialization,
    /// and they're represented the same way in both cooked and uncooked packages, we don't actually have to care abref their
    /// content. The only thing we need to know is how many bytes they are.
    /// </summary>
    public class USerializedImmutableStructProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public int SizeInBytes
        {
            get
            {
                var structDef = (BackingProperty as UStructProperty)?.StructDefinition;
                string structName = structDef?.ObjectName! ?? Tag?.StructName!;

                if (structName == null)
                {
                    throw new Exception($"{nameof(USerializedImmutableStructProperty)}: couldn't find a struct name to use in either {nameof(UProperty)} or {nameof(FPropertyTag)}");
                }

                return structName switch
                {
                    "Color" or "PackedNormal" => 4,
                    "IntPoint" or "Vector2D" => 8,
                    "Rotator" or "Vector" => 12,
                    "Guid" or "LinearColor" or "Plane" or "Quat" or "Vector4" => 16,
                    "FontCharacter" => 21,
                    "TwoVectors" => 24,
                    "Box" => 25,
                    "Matrix" => 64,
                    _ => throw new ArgumentException("Unexpected USerializedImmutableStructProperty tag struct name: " + structDef.ObjectName),
                };
            }
        }

        public override string TagType => "StructProperty";

        public byte[] Data;

        public override void Serialize(IUnrealDataStream stream)
        {
            // The appropriate, generic way to serialize struct properties would be to start from the class definition,
            // use that to find the corresponding UScriptStruct, and iterate its properties, using the archive and struct
            // flags to let us know if we're reading/writing binary data. But since we're only targeting one game, and it
            // has pretty limited usage of immutable structs, we just hardcode everything to make it a lot easier.

            stream.Bytes(ref Data, SizeInBytes);
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedImmutableStructProperty(destArchive, BackingProperty, tag);

            other.Data = Data;

            return other;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }
}
