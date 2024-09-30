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
    public class USerializedObjectProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "ObjectProperty";

        #region Serialized data

        [Index(typeof(UObject))]
        public int ObjectIndex;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref ObjectIndex);
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedObjectProperty(destArchive, BackingProperty, tag);

            other.ObjectIndex = destArchive.MapIndexFromSourceArchive(ObjectIndex, Archive);

            return other;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            dependencyIndices.Add(ObjectIndex);
        }
    }
}
