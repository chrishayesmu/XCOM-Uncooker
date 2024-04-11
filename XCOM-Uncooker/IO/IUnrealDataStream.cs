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
        public void Array<T>(out T[] data, UObject owner = null) where T : IUnrealSerializable, new();

        /// <summary>
        /// A boolean value which takes up 1 byte of space.
        /// </summary>
        public void Bool(out bool value);

        /// <summary>
        /// A boolean value which takes up 4 bytes of space.
        /// </summary>
        public void BoolAsInt32(out bool value);

        /// <summary>
        /// A bulk-serialized array. In addition to serializing the number of elements in the array, bulk arrays
        /// include the size of each element, as they're intended for fixed-size types.
        /// </summary>
        /// <param name="data">Data source or destination for serialization</param>
        /// <param name="elementSize">How many bytes each individual element occupies.</param>
        public void BulkArray<T>(out T[] data, int elementSize, UObject owner = null) where T : IUnrealSerializable, new();

        public void BulkArray(out byte[] data);

        public void BulkArray(out int[] data);

        public void BulkArray(out short[] data);

        public void ByteArray(out byte[] data);

        public void BulkTransactionalArray<T>(out TTransactionalArray<T> data, int elementSize) where T : IUnrealSerializable, new();

        /// <summary>
        /// Serializes a specified number of bytes. Unlike <see cref="ByteArray"/>, the number of bytes is
        /// not output to the stream; the stream will always be advanced by <c>count</c> bytes.
        /// </summary>
        /// <param name="data">Data source or destination for serialization</param>
        /// <param name="count">How many bytes to serialize</param>
        /// <param name="offset">Offset into the <c>data</c> array to begin serialization from</param>
        public void Bytes(out byte[] data, int count, int offset = 0);

        public void Enum32<T>(out T value) where T : Enum;

        public void Enum64<T>(out T value) where T : Enum;

        public void Float32Array(out float[] data);

        public void Float16(out Half value);

        public void Float32(out float value);

        public void GenerationInfo(out FGenerationInfo info);

        public void GenerationInfoArray(out FGenerationInfo[] data);

        public void GuidArray(out Guid[] data);

        public void Guid(out Guid guid);

        public void Int16Array(out short[] data);

        public void Int32Array(out int[] data);

        public void Int16(out short value);

        public void Int32(out int value);

        public void Int64(out long value);

        public void Map(out IDictionary<byte, int> map);

        /// <summary>
        /// Map from an integer to a UBOOL, aka a boolean stored as 4 bytes rather than 1.
        /// </summary>
        public void Map(out IDictionary<int, bool> map);

        public void Map(out IDictionary<int, int> map);

        public void Map(out IDictionary<int, int[]> map);

        public void Map(out IDictionary<long, int> map);

        public void Map(out IDictionary<long, int[]> map);

        public void Map(out IDictionary<FName, int> map);

        public void Map<T>(out IDictionary<int, T[]> map) where T : IUnrealSerializable, new();

        public void Map<T>(out IDictionary<int, T> map) where T : IUnrealSerializable, new();

        /// <summary>
        /// Similar to <see cref="Map(out IDictionary{int, int[]})"/>, but serialized differently. A normal Map is serialized with
        /// each key having a single entry, and that entry's value being the only value for that key. A MultiMap is serialized such
        /// that each key can appear multiple times, and each serialized value is concatenated into the complete value list.
        /// </summary>
        /// <param name="map"></param>
        public void MultiMap(out IDictionary<int, IList<int>> map);

        public void MultiMap<T>(out IDictionary<int, IList<T>> map) where T : IUnrealSerializable, new();

        public void Name(out FName name);

        public void NameArray(out FName[] data);

        public void Object<T>(out T data, UObject owner = null) where T : IUnrealSerializable, new();

        public void PropertyTag(out FPropertyTag tag);

        public void PushedState(out FPushedState state);

        public void PushedStateArray(out FPushedState[] data);

        public void String(out string value);

        public void StringArray(out string[] data);

        public void ThumbnailMetadata(out FThumbnailMetadata metadata);

        public void UInt8(out byte value);

        public void UInt16Array(out ushort[] data);

        public void UInt16(out ushort value);

        public void UInt32(out uint value);

        public void UInt64(out ulong value);
    }
}
