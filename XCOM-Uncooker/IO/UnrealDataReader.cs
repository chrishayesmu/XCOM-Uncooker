using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using XCOM_Uncooker.Unreal.Physical;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.IO
{
    public class UnrealDataReader : Stream, IUnrealDataStream
    {
        /// <summary>
        /// The archive which this stream is operating in the context of. This should be set as soon as
        /// possible, because its data is necessary for understanding how to serialize some types.
        /// </summary>
        public FArchive? Archive;

        public bool IsRead => true;
        public bool IsWrite => false;

        private const int MaxStackAllocSize = 512;

        private readonly Stream _stream;

        public UnrealDataReader(Stream stream)
        {
            _stream = stream;
        }

        #region Stream class overrides
        public override bool CanRead => _stream.CanRead;

        public override bool CanSeek => _stream.CanSeek;

        public override bool CanWrite => _stream.CanWrite;

        public override long Length => _stream.Length;

        public override long Position { get => _stream.Position; set => _stream.Position = value; }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }
        #endregion

        public void SkipBytes(int numBytes)
        {
            _stream.Seek(numBytes, SeekOrigin.Current);
        }

        public void Bool(out bool value)
        {
            UInt8(out byte byteValue);

            value = byteValue > 0;
        }

        public void BoolAsInt32(out bool value)
        {
            Int32(out int boolInt);

            value = boolInt > 0;
        }

        public void Bytes(out byte[] data, int count, int offset = 0)
        {
            data = new byte[count];

            Read(data, offset, count);
        }

        public void Enum32<T>(out T value) where T: Enum
        {
            UInt32(out uint enumAsUInt);
            value = (T) Enum.ToObject(typeof(T), enumAsUInt);
        }

        public void Enum64<T>(out T value) where T : Enum
        {
            UInt64(out ulong enumAsULong);
            value = (T) Enum.ToObject(typeof(T), enumAsULong);
        }

        public void Float32(out float value)
        {
            Span<byte> buffer = stackalloc byte[4];

            _stream.Read(buffer);

            value = MemoryMarshal.Read<float>(buffer);
        }

        public void GenerationInfo(out FGenerationInfo info)
        {
            info = default;
            Span<byte> buffer = stackalloc byte[12];

            _stream.Read(buffer);

            info.ExportCount = MemoryMarshal.Read<int>(buffer.Slice(0, 4));
            info.NameCount = MemoryMarshal.Read<int>(buffer.Slice(4, 4));
            info.NetObjectCount = MemoryMarshal.Read<int>(buffer.Slice(8, 4));
        }

        public void GenerationInfoArray(out FGenerationInfo[] data)
        {
            Int32(out int length);
            data = new FGenerationInfo[length];

            for (int i = 0; i < length; i++)
            {
                GenerationInfo(out data[i]);
            }
        }

        public void Guid(out Guid guid)
        {
            Span<byte> buffer = stackalloc byte[16];

            _stream.Read(buffer);

            guid = new Guid(buffer);
        }

        public void Int16Array(out short[] data)
        {
            Int32(out int length);
            data = new short[length];

            for (int i = 0; i < length; i++)
            {
                Int16(out data[i]);
            }
        }

        public void Int32Array(out int[] data)
        {
            Int32(out int length);
            data = new int[length];

            for (int i = 0; i < length; i++)
            {
                Int32(out data[i]);
            }
        }

        public void Int16(out short value)
        {
            Span<byte> buffer = stackalloc byte[2];

            _stream.Read(buffer);

            value = MemoryMarshal.Read<short>(buffer);
        }

        public void Int32(out int value)
        {
            Span<byte> buffer = stackalloc byte[4];

            _stream.Read(buffer);

            value = MemoryMarshal.Read<int>(buffer);
        }

        public void Int64(out long value)
        {
            Span<byte> buffer = stackalloc byte[8];

            _stream.Read(buffer);

            value = MemoryMarshal.Read<long>(buffer);
        }

        public void Int32ToInt32Map(out IDictionary<int, int> map)
        {
            Int32(out int numEntries);

            map = new Dictionary<int, int>(numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                Int32(out int key);
                Int32(out int value);

                map.Add(key, value);
            }
        }

        public void Name(out FName name)
        {
            name = new FName();

            Int32(out name.Index);
            Int32(out name.Suffix);
            name.Archive = this.Archive!;
        }

        public void NameArray(out FName[] data)
        {
            Int32(out int size);

            data = new FName[size];

            for (int i = 0; i < size; i++)
            {
                Name(out data[i]);
            }
        }

        public void NameToIntMap(out IDictionary<FName, int> map)
        {
            Int32(out int numEntries);

            map = new Dictionary<FName, int>(numEntries);

            for (int i = 0; i < numEntries; i++)
            {
                Name(out FName name);
                Int32(out int value);

                map.Add(name, value);
            }
        }


        public void PropertyTag(out FPropertyTag tag)
        {
            tag = new FPropertyTag();

            Name(out tag.Name);

            if (tag.Name.IsNone())
            {
                return;
            }

            Name(out tag.Type);
            Int32(out tag.Size);
            Int32(out tag.ArrayIndex);

            if (tag.Type == "BoolProperty")
            {
                UInt8(out byte byteVal);
                tag.BoolVal = byteVal > 0;
            }
            else if (tag.Type == "ByteProperty")
            {
                Name(out tag.EnumName);
            }
            else if (tag.Type == "StructProperty")
            {
                Name(out tag.StructName);
            }
        }

        public void PushedState(out FPushedState state)
        {
            state = default;

            Int32(out state.State);
            Int32(out state.Node);
            Int32(out state.Offset);
        }

        public void PushedStateArray(out FPushedState[] data)
        {
            Int32(out int length);

            data = new FPushedState[length];

            for (int i = 0; i < length; i++)
            {
                PushedState(out data[i]);
            }
        }

        public void String(out string value)
        {
            Int32(out int numChars);

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

        public void StringArray(out string[] data)
        {
            Int32(out int length);

            data = new string[length];

            for (int i = 0; i < length; i++)
            {
                String(out data[i]);
            }
        }

        public void ThumbnailMetadata(out FThumbnailMetadata metadata)
        {
            metadata = default;

            String(out metadata.ClassName);
            String(out metadata.ObjectPathWithoutPackageName);
            Int32(out metadata.FileOffset);
        }

        public void UInt8(out byte value)
        {
            value = (byte) ReadByte();
        }

        public void UInt16(out ushort value)
        {
            Span<byte> buffer = stackalloc byte[2];

            _stream.Read(buffer);

            value = MemoryMarshal.Read<ushort>(buffer);
        }

        public void UInt32(out uint value)
        {
            Span<byte> buffer = stackalloc byte[4];

            _stream.Read(buffer);

            value = MemoryMarshal.Read<uint>(buffer);
        }

        public void UInt64(out ulong value)
        {
            Span<byte> buffer = stackalloc byte[8];

            _stream.Read(buffer);

            value = MemoryMarshal.Read<ulong>(buffer);
        }
    }
}
