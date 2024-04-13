using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Textures
{
    public struct FTexture2DMipMap : IUnrealSerializable
    {
        public FTexture2DMipMap() { }

        #region Serialized data

        public FUntypedBulkData Data = new FUntypedBulkData();
        public int SizeX;
        public int SizeY;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            Data.Serialize(stream);
            stream.Int32(ref SizeX);
            stream.Int32(ref SizeY);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FTexture2DMipMap) sourceObj;

            Data = other.Data;
            SizeX = other.SizeX;
            SizeY = other.SizeY;
        }
    }

    public class UTexture2D(FArchive archive, FObjectTableEntry tableEntry) : UTexture(archive, tableEntry)
    {
        #region Serialized data

        public FTexture2DMipMap[] Mips;

        public Guid TextureFileCacheGuid;

        public FTexture2DMipMap[] CachedPVRTCMips;

        public byte[] UnknownData;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        { 
            base.Serialize(stream);

            stream.Array(ref Mips);
            stream.Guid(ref TextureFileCacheGuid);
            stream.Array(ref CachedPVRTCMips);

#if DEBUG
            int bytesRemaining = (int) (ExportTableEntry.SerialOffset + ExportTableEntry.SerialSize - stream.Position);

            // Texture2D has 8 bytes left, LightMapTexture2D has 12
            if (bytesRemaining != 8 && bytesRemaining != 12)
            {
                Debugger.Break();
            }
#endif
      
            // Not sure what these 8 bytes are, but they seem to be in every texture
            stream.Bytes(ref UnknownData, 8);
        }
    }
}
