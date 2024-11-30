using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.Unreal.SerializedProperties;

namespace UnrealArchiveLibrary.Unreal.Intrinsic.Core.Properties
{
    /// <summary>
    /// Represents a UE3 dynamic string. The property has no additional metadata, since
    /// anything there is to know about it will be determined at runtime.
    /// </summary>
    public class UStrProperty(FArchive archive, FObjectTableEntry tableEntry) : UProperty(archive, tableEntry)
    {
        public override USerializedProperty CreateSerializedProperty(FArchive archive, FPropertyTag? tag)
        {
            return new USerializedStringProperty(archive, this, tag);
        }
    }
}
