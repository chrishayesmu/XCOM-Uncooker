using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnrealArchiveLibrary.Unreal;
using UnrealArchiveLibrary.Unreal.SerializedProperties;

namespace UnrealArchiveLibrary.IO
{
    public class UnrealDataReader(Stream stream) : IUnrealDataStream
    {
        public bool IsRead => true;
        public bool IsWrite => false;

        private const int MaxStackAllocSize = 512;

        public FArchive? Archive { get; set; }

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

        public void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        #endregion

        public void SkipBytes(int numBytes)
        {
            _stream.Seek(numBytes, SeekOrigin.Current);

#if DEBUG
            if (_stream.Position > _stream.Length)
            {
                throw new Exception($"Stream overrun: position = {_stream.Position}, length = {_stream.Length}");
            }
#endif
        }

        public void Array<T>(ref T[] data, UObject owner = null) where T : IUnrealSerializable, new()
        {
            int arraySize = 0;
            Int32(ref arraySize);

#if DEBUG
            if (arraySize > 100000)
            {
                throw new Exception("");
            }
#endif

            data = new T[arraySize];

            for (int i = 0; i < arraySize; i++)
            {
                data[i] = new T();
                data[i].Owner = owner;
                data[i].Serialize(this);
            }
        }

        public void Bool(ref bool value)
        {
            byte byteValue = 0;
            UInt8(ref byteValue);

#if DEBUG
            if (byteValue != 0 && byteValue != 1)
            {
                Debugger.Break();
            }
#endif

            value = byteValue > 0;
        }

        public void BoolAsInt32(ref bool value)
        {
            int boolInt = 0;
            Int32(ref boolInt);

#if DEBUG
            if (boolInt != 0 && boolInt != 1)
            {
                // TODO add this back in and debug
                // Debugger.Break();
            }
#endif

            value = boolInt > 0;
        }

        public void BulkArray<T>(ref T[] data, int elementSize, UObject owner = null) where T : IUnrealSerializable, new()
        {
            int actualElementSize = 0;
            Int32(ref actualElementSize);

#if DEBUG
            if (elementSize != actualElementSize)
            {
                throw new Exception($"Expected element size of {elementSize}, found {actualElementSize}");
            }
#endif

            Array(ref data, owner);
        }

        public void BulkArray(ref int[] data)
        {
            int elementSize = 0;
            Int32(ref elementSize);

#if DEBUG
            if (elementSize != 4)
            {
                throw new Exception($"Expected element size of 4, found {elementSize}");
            }
#endif

            Int32Array(ref data);
        }

        public void BulkArray(ref short[] data)
        {
            int elementSize = 0;
            Int32(ref elementSize);

#if DEBUG
            if (elementSize != 2)
            {
                throw new Exception($"Expected element size of 2, found {elementSize}");
            }
#endif

            Int16Array(ref data);
        }

        public void BulkTransactionalArray<T>(ref TTransactionalArray<T> data, int elementSize) where T : IUnrealSerializable, new()
        {
            data = new TTransactionalArray<T>();

            int actualElementSize = 0;
            Int32(ref actualElementSize);

#if DEBUG
            if (elementSize != actualElementSize)
            {
                throw new Exception($"Expected element size of {elementSize}, found {actualElementSize}");
            }
#endif

            data.Serialize(this);
        }

        public void ByteArray(ref byte[] data)
        {
            int length = 0;
            Int32(ref length);
            data = new byte[length];
            Read(data, 0, length);
        }

        public void Bytes(ref byte[] data, int count, int offset = 0)
        {
            data = new byte[count];

            Read(data, offset, count);
        }

        public byte[] CompressedData(ECompressionMethod compressionMethod)
        {
            uint signature = 0;
            int chunkSize = 0, compressedSize = 0, uncompressedSize = 0;

            UInt32(ref signature);

            if (signature != FArchive.UNREAL_SIGNATURE)
            {
                throw new Exception("Compressed data does not appear to be valid (signature mismatch)");
            }

            Int32(ref chunkSize);
            Int32(ref compressedSize);
            Int32(ref uncompressedSize);

            int numCompressedBlocks = (uncompressedSize + chunkSize - 1) / chunkSize;
            FCompressedChunkInfo[] chunkInfo = new FCompressedChunkInfo[numCompressedBlocks];
            int largestCompressedSize = 0;

            for (int i = 0; i < numCompressedBlocks; i++)
            {
                Object(ref chunkInfo[i]);

                if (chunkInfo[i].CompressedSize > largestCompressedSize)
                {
                    largestCompressedSize = chunkInfo[i].CompressedSize;
                }
            }

            byte[] outputData = new byte[uncompressedSize];
            byte[] readBuffer = new byte[largestCompressedSize];
            int outputPos = 0;

            for (int i = 0; i < numCompressedBlocks; i++)
            {
                Read(readBuffer, 0, chunkInfo[i].CompressedSize);
                IOUtils.Decompress(readBuffer, chunkInfo[i].UncompressedSize, outputData, ref outputPos, compressionMethod);
            }

#if DEBUG
            if (outputPos != uncompressedSize)
            {
                Debugger.Break();
            }
#endif

            return outputData;
        }

        public void Enum32<T>(ref T value) where T : Enum
        {
            uint enumAsUInt = 0;
            UInt32(ref enumAsUInt);
            value = (T) Enum.ToObject(typeof(T), enumAsUInt);
        }

        public void Enum64<T>(ref T value) where T : Enum
        {
            ulong enumAsULong = 0;
            UInt64(ref enumAsULong);
            value = (T) Enum.ToObject(typeof(T), enumAsULong);
        }

        public void Float32Array(ref float[] data)
        {
            int length = 0;
            Int32(ref length);
            data = new float[length];

#if DEBUG
            if (length > 100000)
            {
                throw new Exception($"Array length too long, almost certainly a data read error: {length}");
            }
#endif

            for (int i = 0; i < length; i++)
            {
                Float32(ref data[i]);
            }
        }

        public void Float16(ref Half value)
        {
            Span<byte> buffer = stackalloc byte[2];

            _stream.Read(buffer);

            value = MemoryMarshal.Read<Half>(buffer);
        }

        public void Float32(ref float value)
        {
            Span<byte> buffer = stackalloc byte[4];

            _stream.Read(buffer);

            value = MemoryMarshal.Read<float>(buffer);
        }

        public void GuidArray(ref Guid[] data)
        {
            int length = 0;
            Int32(ref length);
            data = new Guid[length];

            for (int i = 0; i < length; i++)
            {
                Guid(ref data[i]);
            }
        }

        public void Guid(ref Guid guid)
        {
            Span<byte> buffer = stackalloc byte[16];

            _stream.Read(buffer);

            guid = new Guid(buffer);
        }

        public void Int16Array(ref short[] data)
        {
            int length = 0;
            Int32(ref length);
            data = new short[length];

            for (int i = 0; i < length; i++)
            {
                Int16(ref data[i]);
            }
        }

        public void Int32Array(ref int[] data)
        {
            int length = 0;
            Int32(ref length);
            data = new int[length];

            for (int i = 0; i < length; i++)
            {
                Int32(ref data[i]);
            }
        }

        public void Int16(ref short value)
        {
            Span<byte> buffer = stackalloc byte[2];

            _stream.Read(buffer);

            value = MemoryMarshal.Read<short>(buffer);
        }

        public void Int32(ref int value)
        {
            Span<byte> buffer = stackalloc byte[4];

            _stream.Read(buffer);

            value = MemoryMarshal.Read<int>(buffer);
        }

        public void Int64(ref long value)
        {
            Span<byte> buffer = stackalloc byte[8];

            _stream.Read(buffer);

            value = MemoryMarshal.Read<long>(buffer);
        }
    
        public void Map(ref IDictionary<byte, int> map)
        {
            int numEntries = 0;
            Int32(ref numEntries);

            map = new Dictionary<byte, int>(numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                byte key = 0;
                int value = 0;
                UInt8(ref key);
                Int32(ref value);

                map.Add(key, value);
            }
        }

        public void Map(ref IDictionary<int, bool> map)
        {
            int numEntries = 0;
            Int32(ref numEntries);

            map = new Dictionary<int, bool>(numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                int key = 0;
                bool value = false;
                Int32(ref key);
                BoolAsInt32(ref value);

                map.Add(key, value);
            }
        }

        public void Map(ref IDictionary<int, int> map)
        {
            int numEntries = 0;
            Int32(ref numEntries);

            map = new Dictionary<int, int>(numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                int key = 0, value = 0;
                Int32(ref key);
                Int32(ref value);

                map.Add(key, value);
            }
        }

        public void Map(ref IDictionary<int, int[]> map)
        {
            int numEntries = 0;
            Int32(ref numEntries);

            map = new Dictionary<int, int[]>(numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                int key = 0;
                int[] value = [];
                Int32(ref key);
                Int32Array(ref value);

                if (map.ContainsKey(key))
                {
                    // this should be very rare or else we would just be using lists instead
                    map[key] = map[key].Concat(value).ToArray();
                }
                else
                {
                   map.Add(key, value);
                }
            }
        }

        public void Map(ref IDictionary<long, int> map)
        {
            int numEntries = 0;
            Int32(ref numEntries);

            map = new Dictionary<long, int>(numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                long key = 0;
                int value = 0;
                Int64(ref key);
                Int32(ref value);

                map.Add(key, value);
            }
        }

        public void Map(ref IDictionary<long, int[]> map)
        {
            int numEntries = 0;
            Int32(ref numEntries);

            map = new Dictionary<long, int[]>(numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                long key = 0;
                int[] value = [];
                Int64(ref key);
                Int32Array(ref value);

                map.Add(key, value);
            }
        }

        public void Map(ref IDictionary<FName, int> map)
        {
            int numEntries = 0;
            Int32(ref numEntries);

            map = new Dictionary<FName, int>(numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                FName name = default;
                int value = 0;
                Name(ref name);
                Int32(ref value);

                map.Add(name, value);
            }
        }

        public void Map(ref IDictionary<FName, Guid> map)
        {
            int numEntries = 0;
            Int32(ref numEntries);

            map = new Dictionary<FName, Guid>(numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                FName name = default;
                Guid value = default;
                Name(ref name);
                Guid(ref value);

                map.Add(name, value);
            }
        }

        public void Map<T>(ref IDictionary<int, T[]> map) where T : IUnrealSerializable, new()
        {
            map = new Dictionary<int, T[]>();

            int numEntries = 0;
            Int32(ref numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                int key = 0;
                T[] data = [];
                Int32(ref key);
                Array(ref data);

                map.Add(key, data);
            }
        }

        public void Map<T>(ref IDictionary<int, T> map) where T : IUnrealSerializable, new()
        {
            map = new Dictionary<int, T>();

            int numEntries = 0;
            Int32(ref numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                int key = 0;
                Int32(ref key);

                T value = new T();
                value.Serialize(this);

                map.Add(key, value);
            }
        }

        public void Map<K, V>(ref IDictionary<K, V> map) where K : IUnrealSerializable, new()
                                                         where V : IUnrealSerializable, new()
        {
            map = new Dictionary<K, V>();

            int numEntries = 0;
            Int32(ref numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                K key = new K();
                key.Serialize(this);

                V value = new V();
                value.Serialize(this);

                map.Add(key, value);
            }
        }

        public void MultiMap(ref IDictionary<int, IList<int>> map)
        {
            map = new Dictionary<int, IList<int>>();

            int numEntries = 0;
            Int32(ref numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                int key = 0, value = 0;
                Int32(ref key);
                Int32(ref value);

                if (!map.TryGetValue(key, out IList<int> valueList))
                {
                    valueList = new List<int>();
                    map.Add(key, valueList);
                }

                valueList.Add(value);
            }
        }

        public void MultiMap<T>(ref IDictionary<int, IList<T>> map) where T : IUnrealSerializable, new()
        {
            map = new Dictionary<int, IList<T>>();

            int numEntries = 0;
            Int32(ref numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                int key = 0;
                Int32(ref key);

                if (!map.TryGetValue(key, out IList<T> valueList))
                {
                    valueList = new List<T>();
                    map.Add(key, valueList);
                }

                T value = new T();
                value.Serialize(this);

                valueList.Add(value);
            }
        }

        public void Name(ref FName name)
        {
            name = new FName();

            Int32(ref name.Index);
            Int32(ref name.Suffix);
            name.Archive = this.Archive!;
        }

        public void NameArray(ref FName[] data)
        {
            int size = 0;
            Int32(ref size);

            data = new FName[size];

            for (int i = 0; i < size; i++)
            {
                Name(ref data[i]);
            }
        }

        public void Object<T>(ref T data, UObject owner = null) where T : IUnrealSerializable, new()
        {
            data = new T();
            data.Owner = owner;
            data.Serialize(this);
        }

        public void String(ref string value)
        {
            int numChars = 0;
            Int32(ref numChars);

            if (numChars == 0)
            {
                value = "";
                return;
            }

            // If the string's length is positive, it indicates the string is stored as ASCII; otherwise,
            // it's been stored as little-endian UTF-16
            Encoding encoding = Encoding.ASCII;
            int charSize = 1;

            if (numChars < 0)
            {
                encoding = Encoding.Unicode;
                charSize = 2;

                numChars *= -1;
            }

            int numBytes = numChars * charSize;
            Span<byte> buffer = numBytes <= MaxStackAllocSize ? stackalloc byte[numBytes] : new byte[numBytes];

            _stream.Read(buffer);

            // Drop the last character, we don't need a null terminator
            value = Encoding.ASCII.GetString(buffer.Slice(0, numBytes - charSize));
        }

        public void StringArray(ref string[] data)
        {
            int length = 0;
            Int32(ref length);

            data = new string[length];

            for (int i = 0; i < length; i++)
            {
                String(ref data[i]);
            }
        }

        public void UInt8(ref byte value)
        {
            value = (byte) _stream.ReadByte();
        }

        public void UInt16Array(ref ushort[] data)
        {
            int length = 0;
            Int32(ref length);

            data = new ushort[length];

            for (int i = 0; i < length; i++)
            {
                UInt16(ref data[i]);
            }
        }

        public void UInt16(ref ushort value)
        {
            Span<byte> buffer = stackalloc byte[2];

            _stream.Read(buffer);

            value = MemoryMarshal.Read<ushort>(buffer);
        }

        public void UInt32(ref uint value)
        {
            Span<byte> buffer = stackalloc byte[4];

            _stream.Read(buffer);

            value = MemoryMarshal.Read<uint>(buffer);
        }

        public void UInt64(ref ulong value)
        {
            Span<byte> buffer = stackalloc byte[8];

            _stream.Read(buffer);

            value = MemoryMarshal.Read<ulong>(buffer);
        }
    }
}
