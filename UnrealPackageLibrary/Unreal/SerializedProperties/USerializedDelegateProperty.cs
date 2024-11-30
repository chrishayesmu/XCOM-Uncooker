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
    public class USerializedDelegateProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "DelegateProperty";

        public override bool HasDefaultValueForType => Value.ObjectIndex == 0;

        #region Serialized data

        public FScriptDelegate Value = new FScriptDelegate(archive);

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            Value.Serialize(stream);
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedDelegateProperty(destArchive, BackingProperty, tag);

            other.Value.CloneFromOtherArchive(Value);

            return other;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            Value.PopulateDependencies(dependencyIndices);
        }
    }
}
