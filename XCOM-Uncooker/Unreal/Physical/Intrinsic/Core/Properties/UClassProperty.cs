using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties
{
    public class UClassProperty(FArchive archive, FObjectTableEntry tableEntry) : UObjectProperty(archive, tableEntry)
    {
        #region Serialized data

        /// <summary>
        /// If this property has a type constraint, this is the index of the class specified
        /// in the constraint. For example:
        /// 
        /// <code>
        /// var class MyClass;
        /// var class&lt;Console&gt; ConsoleClass;
        /// </code>
        /// 
        /// Here, <c>MyClass</c> has no type constraint, and <c>ClassBound</c> will be 0 (indicating <c>None</c>).
        /// <c>ConsoleClass</c> is constrained to <c>Console</c> and its subclasses, so <c>ClassBound</c> will be
        /// the object index of the <c>Console</c> class definition.
        /// </summary>
        [Index(typeof(UClass))]
        public int ClassBound;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(ref ClassBound);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UClassProperty other = (UClassProperty) sourceObj;

            ClassBound = Archive.MapIndexFromSourceArchive(other.ClassBound, other.Archive);
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            dependencyIndices.Add(ClassBound);
        }
    }
}
