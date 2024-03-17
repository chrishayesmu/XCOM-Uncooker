using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public void SkipBytes(int numBytes);

        public void Bool(out bool value);

        public void BoolAsInt32(out bool value);

        public void Bytes(out byte[] data, int count, int offset = 0);

        public void Enum32<T>(out T value) where T : Enum;

        public void Enum64<T>(out T value) where T : Enum;

        public void Float32(out float value);

        public void GenerationInfo(out FGenerationInfo info);

        public void GenerationInfoArray(out FGenerationInfo[] data);

        public void Guid(out Guid guid);

        public void Int16Array(out short[] data);

        public void Int32Array(out int[] data);

        public void Int16(out short value);

        public void Int32(out int value);

        public void Int64(out long value);

        public void Int32ToInt32Map(out IDictionary<int, int> map);

        public void Name(out FName name);

        public void NameArray(out FName[] data);

        public void NameToIntMap(out IDictionary<FName, int> map);

        public void PropertyTag(out FPropertyTag tag);

        public void PushedState(out FPushedState state);

        public void PushedStateArray(out FPushedState[] data);

        public void String(out string value);

        public void StringArray(out string[] data);

        public void ThumbnailMetadata(out FThumbnailMetadata metadata);

        public void UInt8(out byte value);

        public void UInt16(out ushort value);

        public void UInt32(out uint value);

        public void UInt64(out ulong value);
    }
}
