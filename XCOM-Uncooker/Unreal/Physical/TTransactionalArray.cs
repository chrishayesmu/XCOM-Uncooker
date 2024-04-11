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
            stream.Int32(out Owner);
            stream.Array(out Data);
        }
    }
}
