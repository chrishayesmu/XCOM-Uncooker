using Microsoft.Extensions.Logging;
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
    // TODO this class may be entirely unused
    public class USerializedMapProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "MapProperty";

        public override void Serialize(IUnrealDataStream stream)
        {
            if (Tag.HasValue && Tag.Value.Size > 0)
            {
                archive.Log.LogWarning("Asked to serialize {TagSize} bytes, but we don't know how to serialize this", Tag.Value.Size);
            }
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedMapProperty(destArchive, BackingProperty, tag);

            return other;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }
}
