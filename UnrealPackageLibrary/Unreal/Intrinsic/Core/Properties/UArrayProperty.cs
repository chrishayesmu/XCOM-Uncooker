using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.SerializedProperties;

namespace UnrealArchiveLibrary.Unreal.Intrinsic.Core.Properties
{
    /// <summary>
    /// Represents a property which is a dynamic array. Static arrays are a <see cref="UProperty"/> corresponding
    /// to their data type, with <see cref="UProperty.ArrayDim"/> > 1.
    /// </summary>
    public class UArrayProperty(FArchive archive, FObjectTableEntry tableEntry) : UProperty(archive, tableEntry)
    {
        public UProperty Inner => (UProperty) Archive.GetObjectByIndex(InnerProperty);

        #region Serialized data

        /// <summary>
        /// Index of the property type which is contained within the dynamic array.
        /// </summary>
        [Index(typeof(UProperty))]
        public int InnerProperty;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(ref InnerProperty);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UArrayProperty other = (UArrayProperty)sourceObj;

            InnerProperty = Archive.MapIndexFromSourceArchive(other.InnerProperty, other.Archive);
        }

        public override USerializedProperty CreateSerializedProperty(FArchive archive, FPropertyTag? tag)
        {
            return new USerializedArrayProperty(archive, this, tag);
        }
    }
}
