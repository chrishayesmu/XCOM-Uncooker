using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.Unreal;
using XCOM_Uncooker.Unreal.Physical;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.IO
{
    /// <summary>
    /// Interface for reading or writing an <see cref="FArchive" /> file. Whether reading or writing is occurring
    /// is up to the implementing class.
    /// </summary>
    public interface IUnrealDataStream
    {
        public long Position { get; }

        public bool IsRead { get; }

        public bool IsWrite { get; }

        /// <summary>
        /// Advances the stream by <c>numBytes</c> bytes. When reading, this results in simply moving the stream position;
        /// when writing, <c>numBytes</c> bytes with value 0 are written.
        /// </summary>
        /// <param name="numBytes"></param>
        public void SkipBytes(int numBytes);

        /// <summary>
        /// Arbitrary array type. Serialized, the first 4 bytes will be the number of elements in the array.
        /// </summary>
        public void Array<T>(ref T[] data, UObject owner = null) where T : IUnrealSerializable, new();

        /// <summary>
        /// A boolean value which takes up 1 byte of space.
        /// </summary>
        public void Bool(ref bool value);

        /// <summary>
        /// A boolean value which takes up 4 bytes of space.
        /// </summary>
        public void BoolAsInt32(ref bool value);

        /// <summary>
        /// A bulk-serialized array. In addition to serializing the number of elements in the array, bulk arrays
        /// include the size of each element, as they're intended for fixed-size types.
        /// </summary>
        /// <param name="data">Data source or destination for serialization</param>
        /// <param name="elementSize">How many bytes each individual element occupies.</param>
        public void BulkArray<T>(ref T[] data, int elementSize, UObject owner = null) where T : IUnrealSerializable, new();

        public void BulkArray(ref byte[] data);

        public void BulkArray(ref int[] data);

        public void BulkArray(ref short[] data);

        public void ByteArray(ref byte[] data);

        public void BulkTransactionalArray<T>(ref TTransactionalArray<T> data, int elementSize) where T : IUnrealSerializable, new();

        /// <summary>
        /// Serializes a specified number of bytes. Unlike <see cref="ByteArray"/>, the number of bytes is
        /// not output to the stream; the stream will always be advanced by <c>count</c> bytes.
        /// </summary>
        /// <param name="data">Data source or destination for serialization</param>
        /// <param name="count">How many bytes to serialize</param>
        /// <param name="offset">Offset into the <c>data</c> array to begin serialization from</param>
        public void Bytes(ref byte[] data, int count, int offset = 0);

        public void Enum32<T>(ref T value) where T : Enum;

        public void Enum64<T>(ref T value) where T : Enum;

        public void Float32Array(ref float[] data);

        public void Float16(ref Half value);

        public void Float32(ref float value);

        public void GuidArray(ref Guid[] data);

        public void Guid(ref Guid guid);

        public void Int16Array(ref short[] data);

        public void Int32Array(ref int[] data);

        public void Int16(ref short value);

        public void Int32(ref int value);

        public void Int64(ref long value);

        public void Map(ref IDictionary<byte, int> map);

        /// <summary>
        /// Map from an integer to a UBOOL, aka a boolean stored as 4 bytes rather than 1.
        /// </summary>
        public void Map(ref IDictionary<int, bool> map);

        public void Map(ref IDictionary<int, int> map);

        public void Map(ref IDictionary<int, int[]> map);

        public void Map(ref IDictionary<long, int> map);

        public void Map(ref IDictionary<long, int[]> map);

        public void Map(ref IDictionary<FName, int> map);

        public void Map<T>(ref IDictionary<int, T[]> map) where T : IUnrealSerializable, new();

        public void Map<T>(ref IDictionary<int, T> map) where T : IUnrealSerializable, new();

        /// <summary>
        /// Similar to <see cref="Map(ref IDictionary{int, int[]})"/>, but serialized differently. A normal Map is serialized with
        /// each key having a single entry, and that entry's value being the only value for that key. A MultiMap is serialized such
        /// that each key can appear multiple times, and each serialized value is concatenated into the complete value list.
        /// </summary>
        /// <param name="map"></param>
        public void MultiMap(ref IDictionary<int, IList<int>> map);

        public void MultiMap<T>(ref IDictionary<int, IList<T>> map) where T : IUnrealSerializable, new();

        public void Name(ref FName name);

        public void NameArray(ref FName[] data);

        public void Object<T>(ref T data, UObject owner = null) where T : IUnrealSerializable, new();

        public void String(ref string value);

        public void StringArray(ref string[] data);

        public void UInt8(ref byte value);

        public void UInt16Array(ref ushort[] data);

        public void UInt16(ref ushort value);

        public void UInt32(ref uint value);

        public void UInt64(ref ulong value);
    }
}
