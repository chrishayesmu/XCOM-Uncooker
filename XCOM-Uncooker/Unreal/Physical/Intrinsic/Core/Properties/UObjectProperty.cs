using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties
{
    public class UObjectProperty(FArchive archive, FObjectTableEntry tableEntry) : UProperty(archive, tableEntry)
    {
        #region Serialized data

        /// <summary>
        /// Index of the class of the object stored in this property.
        /// </summary>
        [Index(typeof(UClass))]
        public int ObjectClass;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(ref ObjectClass);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UObjectProperty other = (UObjectProperty)sourceObj;

            ObjectClass = Archive.MapIndexFromSourceArchive(other.ObjectClass, other.Archive);
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            dependencyIndices.Add(ObjectClass);
        }

        public override USerializedProperty CreateSerializedProperty(FArchive archive, FPropertyTag? tag)
        {
            return new USerializedObjectProperty(archive, this, tag);
        }
    }
}
