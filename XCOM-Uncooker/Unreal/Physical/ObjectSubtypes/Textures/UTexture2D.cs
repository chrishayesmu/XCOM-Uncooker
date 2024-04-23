using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Actor;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

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
            if (stream.IsRead && stream.Archive.PackageFileSummary.LicenseeVersion > 0)
            {
                int bytesRemaining = (int) (ExportTableEntry.SerialOffset + ExportTableEntry.SerialSize - stream.Position);

                // Texture2D has 8 bytes left, LightMapTexture2D has 12
                if (bytesRemaining != 8 && bytesRemaining != 12)
                {
                    Debugger.Break();
                }
            }
#endif

            // Not sure what these 8 bytes are, but they seem to be in every texture
            // TODO figure this out? may not actually be needed for loading new textures in XCOM anyway
            if (stream.IsRead)
            {
                stream.Bytes(ref UnknownData, 8);
            }
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UTexture2D) sourceObj;

            Mips = other.Mips;
            TextureFileCacheGuid = other.TextureFileCacheGuid;
            CachedPVRTCMips = other.CachedPVRTCMips;
            UnknownData = other.UnknownData;

            // Retrieve texture data from the TFC so it can be part of the UPK
            if (Mips.Length > 0)
            {
                int largestMipsIndex = FindLargestMipsIndex();

                if (largestMipsIndex < 0)
                {
                    return;
                }

                int numElements = Mips[largestMipsIndex].Data.NumElements;
                byte[] textureData;

                if (Mips[largestMipsIndex].Data.BulkDataFlags.HasFlag(EBulkDataFlags.StoreInSeparateFile)) 
                {
                    var nameProp = GetSerializedProperty("TextureFileCacheName") as USerializedNameProperty;

#if DEBUG
                    if (nameProp == null)
                    {
                        throw new Exception($"UTexture2D {FullObjectPath} doesn't have a TextureFileCacheName property!");
                    }
#endif

                    textureData = Archive.ParentLinker.ReadTextureData(nameProp.Value, Mips[largestMipsIndex].Data.Offset, Mips[largestMipsIndex].Data.SizeOnDisk);
                }
                else
                {
                    textureData = Mips[largestMipsIndex].Data.Data;
                }

                SourceArt.BulkDataFlags = Mips[largestMipsIndex].Data.BulkDataFlags;
                SourceArt.SetData(textureData, numElements);
            }
        }

        protected int FindLargestMipsIndex()
        {
            int index = -1;

            for (int i = 0; i < Mips.Length; i++)
            {
                if (Mips[i].Data.SizeOnDisk <= 0 || Mips[i].Data.NumElements <= 0)
                {
                    continue;
                }

                if (index < 0 || Mips[i].SizeX > Mips[index].SizeX)
                {
                    index = i;
                }
            }

            return index;
        }
    }
}
