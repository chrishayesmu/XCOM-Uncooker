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
    public class UMapProperty(FArchive archive, FObjectTableEntry tableEntry) : UProperty(archive, tableEntry)
    {
        #region Serialized data

        /// <summary>
        /// Index of the property type which is used for the keys of this map.
        /// </summary>
        [Index(typeof(UProperty))]
        public int KeyProperty;

        /// <summary>
        /// Index of the property type which is used for the values of this map.
        /// </summary>
        [Index(typeof(UProperty))]
        public int ValueProperty;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(ref KeyProperty);
            stream.Int32(ref ValueProperty);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UMapProperty other = (UMapProperty) sourceObj;

            KeyProperty = Archive.MapIndexFromSourceArchive(other.KeyProperty, other.Archive);
            ValueProperty = Archive.MapIndexFromSourceArchive(other.ValueProperty, other.Archive);
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            dependencyIndices.Add(KeyProperty);
            dependencyIndices.Add(ValueProperty);
        }

        public override USerializedProperty CreateSerializedProperty(FArchive archive, FPropertyTag? tag)
        {
            throw new NotImplementedException();
        }
    }
}
