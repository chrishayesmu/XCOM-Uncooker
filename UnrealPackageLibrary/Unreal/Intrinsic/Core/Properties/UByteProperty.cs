using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.Intrinsic.Core;
using UnrealArchiveLibrary.Unreal.SerializedProperties;

namespace UnrealArchiveLibrary.Unreal.Intrinsic.Core.Properties
{
    public class UByteProperty(FArchive archive, FObjectTableEntry tableEntry) : UProperty(archive, tableEntry)
    {
        #region Serialized data

        /// <summary>
        /// UByteProperty is used for both byte and enum types. If this property holds an enum type,
        /// then the index of the referenced enum type will be in <c>EnumIndex</c>.
        /// </summary>
        [Index(typeof(UEnum))]
        public int EnumIndex;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(ref EnumIndex);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UByteProperty other = (UByteProperty)sourceObj;

            EnumIndex = Archive.MapIndexFromSourceArchive(other.EnumIndex, other.Archive);
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            dependencyIndices.Add(EnumIndex);
        }

        public override USerializedProperty CreateSerializedProperty(FArchive archive, FPropertyTag? tag)
        {
            return new USerializedByteProperty(archive, this, tag);
        }
    }
}
