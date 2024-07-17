using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical
{
    /// <summary>
    /// This class represents a bulk-serialized byte array where the data is typed, but we don't actually care what that type is.
    /// There is not a direct UE3 parallel to this class, but it is meant to be used in place of TArray::BulkSerialize.
    /// </summary>
    public class FByteArrayWithSize : IUnrealSerializable
    {
        #region Serialized data

        public int ElementSize;

        public int NumElements;

        public byte[] Data;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref ElementSize);

            if (ElementSize == 0)
            {
                return;
            }

            stream.Int32(ref NumElements);
            stream.Bytes(ref Data, ElementSize * NumElements);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FByteArrayWithSize) sourceObj;

            ElementSize = other.ElementSize;
            NumElements = other.NumElements;
            Data = other.Data;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }
}
