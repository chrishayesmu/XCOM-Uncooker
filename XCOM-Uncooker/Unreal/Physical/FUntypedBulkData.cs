using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical
{
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

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Enum32(ref BulkDataFlags);
            stream.Int32(ref NumElements);
            stream.Int32(ref SizeOnDisk);
            stream.Int32(ref Offset);

            if (BulkDataFlags.HasFlag(EBulkDataFlags.StoreInSeparateFile))
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FUntypedBulkData) sourceObj;

            BulkDataFlags = other.BulkDataFlags;
            NumElements = other.NumElements;
            SizeOnDisk = other.SizeOnDisk;
            Offset = other.Offset;
            Data = other.Data;
        }
    }
}
