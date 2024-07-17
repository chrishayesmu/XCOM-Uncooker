using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties.ImmutableWhenCooked;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties
{
    public class UStructProperty(FArchive archive, FObjectTableEntry tableEntry) : UProperty(archive, tableEntry)
    {
        /// <summary>
        /// Structs in XCOM: EW which are known to be immutable, both when cooked and uncooked. Since their representation is the same both ways, 
        /// we don't have to bother with understanding their internals; just copy their input data as their output data.
        /// </summary>
        public static readonly string[] ImmutableStructs = ["Box", "Color", "FontCharacter", "Guid", "IntPoint", "LinearColor", "Matrix", "PackedNormal", "Plane", "Quat", "Rotator", "TwoVectors", "Vector", "Vector2D", "Vector4"];

        /// <summary>
        /// Structs in XCOM: EW which are known to be immutablewhencooked. We need to understand the structure of these, so that we can read
        /// them and turn them back into tagged properties for the editor to use.
        /// </summary>
        public static readonly string[] ImmutableWhenCookedStructs = ["ActorReference", "AimComponent", "AimOffsetProfile", "AimTransform", "InstancedStaticMeshInstanceData", "NavReference"];

        public UScriptStruct StructDefinition => (UScriptStruct) Archive.GetObjectByIndex(ScriptStruct);

        #region Serialized data

        /// <summary>
        /// Index of the <see cref="UScriptStruct"/> which is stored in this property.
        /// </summary>
        [Index(typeof(UScriptStruct))]
        public int ScriptStruct;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(ref ScriptStruct);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UStructProperty other = (UStructProperty) sourceObj;

            ScriptStruct = Archive.MapIndexFromSourceArchive(other.ScriptStruct, other.Archive);
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            dependencyIndices.Add(ScriptStruct);
        }

        public override USerializedProperty CreateSerializedProperty(FArchive archive, FPropertyTag? tag)
        {
            string structName = tag?.StructName ?? StructDefinition.ObjectName;

            if (ImmutableStructs.Contains(structName))
            {
                return new USerializedImmutableStructProperty(archive, this, tag);
            }

            return structName switch
            {
                "ActorReference" => new USerializedActorReferenceProperty(archive, this, tag),
                "AimComponent" => new USerializedAimComponentProperty(archive, this, tag),
                "AimOffsetProfile" => new USerializedAimOffsetProfileProperty(archive, this, tag),
                "AimTransform" => new USerializedAimTransformProperty(archive, this, tag),
                "InstancedStaticMeshInstanceData" => new USerializedInstancedStaticMeshInstanceDataProperty(archive, this, tag),
                "NavReference" => new USerializedNavReferenceProperty(archive, this, tag),
                _ => new USerializedStructProperty(archive, this, tag),
            };
        }

        public static bool TryCreateSerializedStructProperty(FArchive archive, FPropertyTag tag, out USerializedProperty prop)
        {
            prop = null;

            if (!tag.IsStructProperty) 
            {
                return false;
            }

            if (ImmutableStructs.Contains(tag.StructName))
            {
                prop = new USerializedImmutableStructProperty(archive, null, tag);
                return true;
            }

#pragma warning disable CS8601 // Possible null reference assignment.
            prop = (string) tag.StructName switch
            {
                "ActorReference" => new USerializedActorReferenceProperty(archive, null, tag),
                "AimComponent" => new USerializedAimComponentProperty(archive, null, tag),
                "AimOffsetProfile" => new USerializedAimOffsetProfileProperty(archive, null, tag),
                "AimTransform" => new USerializedAimTransformProperty(archive, null, tag),
                "InstancedStaticMeshInstanceData" => new USerializedInstancedStaticMeshInstanceDataProperty(archive, null, tag),
                "NavReference" => new USerializedNavReferenceProperty(archive, null, tag),
                _ => null
            };
#pragma warning restore CS8601 // Possible null reference assignment.

            return prop != null;
        }
    }
}
