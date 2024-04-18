using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.Unreal;
using XCOM_Uncooker.Unreal.Physical;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.IO
{
    public class UnrealDataWriter(Stream stream) : IUnrealDataStream
    {
        /// <summary>
        /// The archive which this stream is operating in the context of. This should be set as soon as
        /// possible, because its data is necessary for understanding how to serialize some types.
        /// </summary>
        public FArchive? Archive { get; set; }

        public bool IsRead => false;
        public bool IsWrite => true;

        private readonly Stream _stream = stream;

        #region Stream class overrides

        public bool CanRead => _stream.CanRead;

        public bool CanSeek => _stream.CanSeek;

        public bool CanWrite => _stream.CanWrite;

        public long Length => _stream.Length;

        public long Position { get => _stream.Position; set => _stream.Position = value; }

        public void Dispose()
        {
            _stream.Dispose();
        }

        public void Flush()
        {
            _stream.Flush();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public void SetLength(long value)
        {
            _stream.SetLength(value);
        }
        
        public void Write(Span<byte> buffer)
        {
            _stream.Write(buffer);
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        #endregion

        public void SkipBytes(int numBytes)
        {
            Write(new byte[numBytes]);
        }

        public void Array<T>(ref T[] data, UObject owner = null) where T : IUnrealSerializable, new()
        {
            int arraySize = data.Length;
            Int32(ref arraySize);

            for (int i = 0; i < arraySize; i++)
            {
                data[i].Serialize(this);
            }
        }

        public void Bool(ref bool value)
        {
            byte byteValue = (byte) (value ? 1 : 0);
            UInt8(ref byteValue);
        }

        public void BoolAsInt32(ref bool value)
        {
            int intValue = value ? 1 : 0;
            Int32(ref intValue);
        }

        public void BulkArray<T>(ref T[] data, int elementSize, UObject owner = null) where T : IUnrealSerializable, new()
        {
            Int32(ref elementSize);
            Array(ref data, owner);
        }

        public void BulkArray(ref byte[] data)
        {
            throw new NotImplementedException("Need to persist the element size for bulk byte arrays!");
        }

        public void BulkArray(ref int[] data)
        {
            int elementSize = 4;
            Int32(ref elementSize);
            Int32Array(ref data);
        }

        public void BulkArray(ref short[] data)
        {
            int elementSize = 2;
            Int32(ref elementSize);
            Int16Array(ref data);
        }

        public void BulkTransactionalArray<T>(ref TTransactionalArray<T> data, int elementSize) where T : IUnrealSerializable, new()
        {
            Int32(ref elementSize);
            data.Serialize(this);
        }

        public void ByteArray(ref byte[] data)
        {
            int arraySize = data.Length;
            Int32(ref arraySize);
            Write(data, 0, arraySize);
        }

        public void Bytes(ref byte[] data, int count, int offset = 0)
        {
            Write(data, offset, count);
        }

        public void Enum32<T>(ref T value) where T : Enum
        {
            uint enumAsUInt = (uint) Convert.ChangeType(value, TypeCode.UInt32);
            UInt32(ref enumAsUInt);
        }

        public void Enum64<T>(ref T value) where T : Enum
        {
            ulong enumAsULong = (ulong) Convert.ChangeType(value, TypeCode.UInt64);
            UInt64(ref enumAsULong);
        }

        public void Float32Array(ref float[] data)
        {
            int arraySize = data.Length;
            Int32(ref arraySize);

            for (int i = 0; i < arraySize; i++)
            {
                Float32(ref data[i]);
            }
        }

        public void Float16(ref Half value)
        {
            Span<byte> buffer = stackalloc byte[2];
            MemoryMarshal.Write(buffer, value);

            Write(buffer);
        }

        public void Float32(ref float value)
        {
            Span<byte> buffer = stackalloc byte[4];
            MemoryMarshal.Write(buffer, value);

            Write(buffer);
        }

        public void GuidArray(ref Guid[] data)
        {
            int arraySize = data.Length;
            Int32(ref arraySize);

            for (int i = 0; i < arraySize; i++)
            {
                Guid(ref data[i]);
            }
        }

        public void Guid(ref Guid guid)
        {
            Span<byte> buffer = stackalloc byte[16];

            guid.TryWriteBytes(buffer);

            Write(buffer);
        }

        public void Int16Array(ref short[] data)
        {
            int arraySize = data.Length;
            Int32(ref arraySize);

            for (int i = 0; i < arraySize; i++)
            {
                Int16(ref data[i]);
            }
        }

        public void Int32Array(ref int[] data)
        {
            int arraySize = data.Length;
            Int32(ref arraySize);

            for (int i = 0; i < arraySize; i++)
            {
                Int32(ref data[i]);
            }
        }

        public void Int16(ref short value)
        {
            Span<byte> buffer = stackalloc byte[2];
            MemoryMarshal.Write(buffer, value);

            Write(buffer);
        }

        public void Int32(ref int value)
        {
            Span<byte> buffer = stackalloc byte[4];
            MemoryMarshal.Write(buffer, value);

            Write(buffer);
        }

        public void Int64(ref long value)
        {
            Span<byte> buffer = stackalloc byte[8];
            MemoryMarshal.Write(buffer, value);

            Write(buffer);
        }

        public void Map(ref IDictionary<byte, int> map)
        {
            int numEntries = 0;
            Int32(ref numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                var entry = map.ElementAt(i);
                var key = entry.Key;
                var value = entry.Value;

                UInt8(ref key);
                Int32(ref value);
            }
        }

        public void Map(ref IDictionary<int, bool> map)
        {
            int numEntries = 0;
            Int32(ref numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                var entry = map.ElementAt(i);
                var key = entry.Key;
                var value = entry.Value;

                Int32(ref key);
                BoolAsInt32(ref value);
            }
        }

        public void Map(ref IDictionary<int, int> map)
        {
            int numEntries = 0;
            Int32(ref numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                var entry = map.ElementAt(i);
                var key = entry.Key;
                var value = entry.Value;

                Int32(ref key);
                Int32(ref value);
            }
        }

        public void Map(ref IDictionary<int, int[]> map)
        {
            int numEntries = 0;
            Int32(ref numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                var entry = map.ElementAt(i);
                var key = entry.Key;
                var value = entry.Value;

                Int32(ref key);
                Int32Array(ref value);
            }
        }

        public void Map(ref IDictionary<long, int> map)
        {
            int numEntries = 0;
            Int32(ref numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                var entry = map.ElementAt(i);
                var key = entry.Key;
                var value = entry.Value;

                Int64(ref key);
                Int32(ref value);
            }
        }

        public void Map(ref IDictionary<long, int[]> map)
        {
            int numEntries = 0;
            Int32(ref numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                var entry = map.ElementAt(i);
                var key = entry.Key;
                var value = entry.Value;

                Int64(ref key);
                Int32Array(ref value);
            }
        }

        public void Map(ref IDictionary<FName, int> map)
        {
            int numEntries = 0;
            Int32(ref numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                var entry = map.ElementAt(i);
                var key = entry.Key;
                var value = entry.Value;

                Name(ref key);
                Int32(ref value);
            }
        }

        public void Map<T>(ref IDictionary<int, T[]> map) where T : IUnrealSerializable, new()
        {
            int numEntries = 0;
            Int32(ref numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                var entry = map.ElementAt(i);
                var key = entry.Key;
                var value = entry.Value;

                Int32(ref key);
                Array(ref value);
            }
        }

        public void Map<T>(ref IDictionary<int, T> map) where T : IUnrealSerializable, new()
        {
            int numEntries = 0;
            Int32(ref numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                var entry = map.ElementAt(i);
                var key = entry.Key;
                var value = entry.Value;

                Int32(ref key);
                Object(ref value);
            }
        }

        public void MultiMap(ref IDictionary<int, IList<int>> map)
        {
            // Multimaps only need special handling when read, for accepting repeated keys; for writing
            // they're the same as a regular map of the same type

            int numEntries = 0;
            Int32(ref numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                var entry = map.ElementAt(i);
                var key = entry.Key;
                var value = entry.Value.ToArray();

                Int32(ref key);
                Int32Array(ref value);
            }
        }

        public void MultiMap<T>(ref IDictionary<int, IList<T>> map) where T : IUnrealSerializable, new()
        {
            int numEntries = 0;
            Int32(ref numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                var entry = map.ElementAt(i);
                var key = entry.Key;
                var value = entry.Value.ToArray();

                Int32(ref key);
                Array(ref value);
            }
        }

        public void Name(ref FName name)
        {
            Int32(ref name.Index);
            Int32(ref name.Suffix);
        }

        public void NameArray(ref FName[] data)
        {
            int arraySize = data.Length;
            Int32(ref arraySize);

            for (int i = 0; i < arraySize; i++)
            {
                Name(ref data[i]);
            }
        }

        public void Object<T>(ref T data, UObject owner = null) where T : IUnrealSerializable, new()
        {
            data.Serialize(this);
        }

        public void String(ref string value)
        {
            // A negative string length is used to indicate that a string is UTF-16 encoded rather than ASCII
            bool isAscii = value.All(char.IsAscii);
            int numChars = isAscii ? value.Length : -1 * value.Length;   ;
            Int32(ref numChars);

            Encoding encoding = isAscii ? Encoding.ASCII : Encoding.Unicode;
            byte[] bytes = encoding.GetBytes(value + '\0'); // add null terminator
            Write(bytes);
        }

        public void StringArray(ref string[] data)
        {
            int arraySize = data.Length;
            Int32(ref arraySize);

            for (int i = 0; i < arraySize; i++)
            {
                String(ref data[i]);
            }
        }

        public void UInt8(ref byte value)
        {
            Span<byte> buffer = stackalloc byte[1];
            MemoryMarshal.Write(buffer, value);

            Write(buffer);
        }

        public void UInt16Array(ref ushort[] data)
        {
            int arraySize = data.Length;
            Int32(ref arraySize);

            for (int i = 0; i < arraySize; i++)
            {
                UInt16(ref data[i]);
            }
        }

        public void UInt16(ref ushort value)
        {
            Span<byte> buffer = stackalloc byte[2];
            MemoryMarshal.Write(buffer, value);

            Write(buffer);
        }

        public void UInt32(ref uint value)
        {
            Span<byte> buffer = stackalloc byte[4];
            MemoryMarshal.Write(buffer, value);

            Write(buffer);
        }

        public void UInt64(ref ulong value)
        {
            Span<byte> buffer = stackalloc byte[8];
            MemoryMarshal.Write(buffer, value);

            Write(buffer);
        }
    }
}
