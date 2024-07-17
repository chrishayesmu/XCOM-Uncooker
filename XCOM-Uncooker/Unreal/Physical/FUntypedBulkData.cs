using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical
{
    [Flags]
    public enum EBulkDataFlags
    {
        None                               = 0x00,
        StoreInSeparateFile                = 0x01,
        SerializeCompressedZLIB            = 0x02,
        ForceSingleElementSerialization    = 0x04,
        SingleUse                          = 0x08,
        SerializeCompressedLZO             = 0x10,
        Unused                             = 0x20,
        StoreOnlyPayload                   = 0x40,
        SerializeCompressedLZX             = 0x80
    }

    public class FUntypedBulkData : IUnrealSerializable
    {
        public EBulkDataFlags BulkDataFlags;

        public int NumElements;

        public int SizeOnDisk;

        public int Offset;

        public byte[] Data;

        public bool IsCompressed => BulkDataFlags.HasFlag(EBulkDataFlags.SerializeCompressedZLIB) || BulkDataFlags.HasFlag(EBulkDataFlags.SerializeCompressedLZO) || BulkDataFlags.HasFlag(EBulkDataFlags.SerializeCompressedLZX);

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Enum32(ref BulkDataFlags);
            stream.Int32(ref NumElements);
            stream.Int32(ref SizeOnDisk);

            // When we're uncooking, we need to update any inline bulk data to point to the current file
            // position as its offset; inline data is always immediately following the metadata.
            bool storedSeparately = BulkDataFlags.HasFlag(EBulkDataFlags.StoreInSeparateFile);

            if (stream.IsWrite && !storedSeparately)
            {
                Offset = (int) stream.Position + 4;
            }

            stream.Int32(ref Offset);

            if (storedSeparately)
            {
                // TODO: where the file is depends on the data (e.g. in a TFC for textures)
            }
            else 
            {
                // If the data's not in a separate file, then it will immediately follow the Offset field,
                // so we don't need to bother seeking for it
                stream.Bytes(ref Data, SizeOnDisk);
            }
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FUntypedBulkData) sourceObj;

            BulkDataFlags = other.BulkDataFlags;
            NumElements = other.NumElements;
            SizeOnDisk = other.SizeOnDisk;
            Offset = other.Offset;
            Data = other.Data;
        }

        /// <summary>
        /// Causes this container to decompress its stored data, if the data is compressed and if the data flags
        /// do not include <c>StoreInSeparateFile</c>.
        /// </summary>
        public void Decompress()
        {
            ECompressionMethod compressionMethod = GetCompressionMethod();

            if (compressionMethod == ECompressionMethod.None)
            {
                return;
            }

            var dataAsStream = new UnrealDataReader(new MemoryStream(Data));
            Data = dataAsStream.CompressedData(compressionMethod);

            // Clear any compression flags
            BulkDataFlags &= ~(EBulkDataFlags.SerializeCompressedLZO | EBulkDataFlags.SerializeCompressedLZX | EBulkDataFlags.SerializeCompressedZLIB);

            // Size on disk will change both because of the compression, and because some compression metadata is removed
            SizeOnDisk = Data.Length;
        }

        /// <summary>
        /// Sets the data for this container. This will cause the data to not be marked as <c>StoreInSeparateFile</c>,
        /// and thus during uncooking it will be serialized into the UPK.
        /// </summary>
        /// <param name="data"></param>
        public void SetData(byte[] data, int numElements)
        {
            Data = data;
            BulkDataFlags &= ~(EBulkDataFlags.StoreInSeparateFile);
            NumElements = numElements;
            SizeOnDisk = data.Length;
        }

        private ECompressionMethod GetCompressionMethod()
        {
            if (BulkDataFlags.HasFlag(EBulkDataFlags.SerializeCompressedLZO))
            {
                return ECompressionMethod.LZO;
            }
            else if (BulkDataFlags.HasFlag(EBulkDataFlags.SerializeCompressedLZX))
            {
                return ECompressionMethod.LZX;
            }
            else if (BulkDataFlags.HasFlag(EBulkDataFlags.SerializeCompressedZLIB))
            {
                return ECompressionMethod.ZLIB;
            }
            else
            {
                return ECompressionMethod.None;
            }
        }
    }
}
