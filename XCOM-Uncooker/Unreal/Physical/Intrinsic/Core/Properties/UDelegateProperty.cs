using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties
{
    public class UDelegateProperty(FArchive archive, FObjectTableEntry tableEntry) : UProperty(archive, tableEntry)
    {
        #region Serialized data

        [Index(typeof(UFunction))]
        public int Function;

        [Index(typeof(UFunction))]
        public int SourceDelegate;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(ref Function);
            stream.Int32(ref SourceDelegate);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UDelegateProperty other = (UDelegateProperty) sourceObj;

            Function = Archive.MapIndexFromSourceArchive(other.Function, other.Archive);
            SourceDelegate = Archive.MapIndexFromSourceArchive(other.SourceDelegate, other.Archive);
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            dependencyIndices.Add(Function);
            dependencyIndices.Add(SourceDelegate);
        }

        public override USerializedProperty CreateSerializedProperty(FArchive archive, FPropertyTag? tag)
        {
            return new USerializedDelegateProperty(archive, this, tag);
        }
    }
}
