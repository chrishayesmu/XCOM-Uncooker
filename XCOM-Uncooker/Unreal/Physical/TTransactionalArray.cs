using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical
{
    public class TTransactionalArray<T> : IUnrealSerializable where T : IUnrealSerializable, new()
    {
        #region Serialized data

        [Index(typeof(UObject))]
        public int Owner;

        public T[] Data;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref Owner);
            stream.Array(ref Data);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (TTransactionalArray<T>) sourceObj;

            Owner = destArchive.MapIndexFromSourceArchive(other.Owner, sourceArchive);
            Data = IUnrealSerializable.Clone(other.Data, sourceArchive, destArchive);
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
            dependencyIndices.Add(Owner);
        }
    }
}
