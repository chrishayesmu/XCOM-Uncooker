﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.Intrinsic.Core.Properties;
using UnrealArchiveLibrary.Unreal.SerializedProperties.ImmutableWhenCooked;

namespace UnrealArchiveLibrary.Unreal.SerializedProperties
{
    /// <summary>
    /// This property can represent either a single byte of data, or an enum value. Enum values are a single byte at
    /// runtime, but they are serialized to disk using their enum name (8 bytes) instead of their integer value. This
    /// allows for enum values to be added to or rearranged without invalidating previously-created objects.
    /// </summary>
    public class USerializedByteProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "ByteProperty";

        // TODO: for enum types, we'd need to look up the backing property and check which name is at the 0 index.
        // That sounds cumbersome and slow, so just return false for now, which will sometimes force tag serialization
        // when it's not necessary (but is better than the reverse: skipping necessarily serialization).
        public override bool HasDefaultValueForType => IsEnumType ? false : ByteValue == 0;

        private bool IsEnumType => (BackingProperty as UByteProperty)?.EnumIndex != 0 || (!Tag?.EnumName.IsNone() ?? false);

        #region Serialized data

        public byte ByteValue;

        public FName? EnumValue;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            UByteProperty? prop = BackingProperty as UByteProperty;
            bool isEnumType = prop?.EnumIndex != 0 || (!Tag?.EnumName.IsNone() ?? false);

            if (!isEnumType)
            {
                // With no enum name set, this is an actual byte variable, and will be serialized as such
                stream.UInt8(ref ByteValue);
            }
            else
            {
                stream.Name(ref EnumValue!);
            }
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedByteProperty(destArchive, BackingProperty, tag);

            other.ByteValue = ByteValue;
            other.EnumValue = destArchive.MapNameFromSourceArchive(EnumValue);

            return other;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }
}
