using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.Intrinsic.Core.Properties;
using UnrealArchiveLibrary.Unreal.SerializedProperties.ImmutableWhenCooked;

namespace UnrealArchiveLibrary.Unreal.SerializedProperties
{
    public class USerializedStringProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "StrProperty";

        public override bool HasDefaultValueForType => Value.Length == 0;

        #region Serialized data

        public string Value;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            stream.String(ref Value);
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedStringProperty(destArchive, BackingProperty, tag);

            other.Value = Value;

            return other;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }
}
