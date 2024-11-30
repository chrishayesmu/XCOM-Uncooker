using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.Intrinsic.Core.Properties;

namespace UnrealArchiveLibrary.Unreal.SerializedProperties
{
    public class USerializedStructProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "StructProperty";

        public override bool HasDefaultValueForType => TaggedProperties.All(prop => prop.HasDefaultValueForType);

        #region Serialized data

        public List<USerializedProperty> TaggedProperties = [];
        
        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            // Read the properties like any other props
            var structProp = (UStructProperty) BackingProperty;
            structProp.StructDefinition.SerializeTaggedProperties(TaggedProperties, stream);
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedStructProperty(destArchive, BackingProperty, tag);

            other.TaggedProperties = new List<USerializedProperty>(TaggedProperties.Count);

            for (int i = 0; i < TaggedProperties.Count; i++)
            {
                var clonedProp = TaggedProperties[i].CloneToOtherArchive(destArchive);
                other.TaggedProperties.Add(clonedProp);
            }

            // Clone needed default values; see UObject.CloneFromOtherArchive for explanation
            var structProp = (UStructProperty) BackingProperty;
            foreach (var defaultProp in structProp.StructDefinition.StructDefaultProperties)
            {
                if (!defaultProp.HasDefaultValueForType && !other.TaggedProperties.Any(p => p.Tag?.Name == defaultProp.Tag?.Name))
                {
                    var clonedProp = defaultProp.CloneToOtherArchive(destArchive);
                    other.TaggedProperties.Add(clonedProp);
                }
            }

            return other;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            foreach (var prop in TaggedProperties)
            {
                prop.PopulateDependencies(dependencyIndices);
            }
        }

        public USerializedProperty GetSerializedProperty(string propName)
        {
            foreach (var prop in TaggedProperties)
            {
                if (prop.Tag?.Name == propName)
                {
                    return prop;
                }
            }

            return null;
        }
    }
}
