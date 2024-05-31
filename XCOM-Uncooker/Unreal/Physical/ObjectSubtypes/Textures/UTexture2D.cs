using BCnEncoder.Decoder;
using BCnEncoder.ImageSharp;
using BCnEncoder.Shared;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
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
        private static readonly Logger Log = new Logger(nameof(UTexture2D));

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
            if (stream.IsRead && stream.Archive.PackageFileSummary.LicenseeVersion > 0)
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

            var compressionFormat = GetCompressionFormat();
            var nameProp = GetSerializedProperty("TextureFileCacheName") as USerializedNameProperty;
            string? tfcName = nameProp?.Value?.ToString();

            // Uncooked mipmaps need to be uncompressed in the UPK
            for (int i = 0; i < Mips.Length; i++)
            {
                Mips[i].Data = LoadAndDecompressTextureData(tfcName, compressionFormat, Mips[i].SizeX, Mips[i].SizeY, Mips[i].Data);
            }

            // Retrieve texture data from the TFC so it can be part of the UPK
            if (Mips.Length > 0)
            {
                int largestMipsIndex = FindBestMipsIndex();

                if (largestMipsIndex < 0)
                {
                    return;
                }

                FTexture2DMipMap sourceMipMap = Mips[largestMipsIndex];

                // Occasionally the best mip doesn't match the source art size, because the full size mip was marked as unused
                // during cooking and never committed to disk. In that case we need to update the original size values of the
                // texture, or the UDK will think it's corrupted and fail to load it properly.
                var originalSizeXProp = GetSerializedProperty("OriginalSizeX") as USerializedIntProperty;

                // OriginalSizeX can be null on some textures, like lightmaps
                if (originalSizeXProp != null && originalSizeXProp.Value != sourceMipMap.SizeX)
                {
                    var originalSizeYProp = GetSerializedProperty("OriginalSizeY") as USerializedIntProperty;
                    
                    Log.Verbose($"{FullObjectPath}: couldn't find a mip matching the original size ({originalSizeXProp.Value}, {originalSizeYProp.Value}). Replacing with ({sourceMipMap.SizeX}, {sourceMipMap.SizeY}).");

                    originalSizeXProp.Value = sourceMipMap.SizeX;
                    originalSizeYProp.Value = sourceMipMap.SizeY;
                }

                // Take the largest mip, transform it to PNG, and store it as source art
                var decoder = new BcDecoder();
                var decodedImageData = decoder.DecodeRawToImageRgba32(sourceMipMap.Data.Data, sourceMipMap.SizeX, sourceMipMap.SizeY, compressionFormat);

                // For some reason the red and blue channels appear to be swapped in the source art in the UDK.
                decodedImageData.ProcessPixelRows(accessor =>
                {
                    for (int y = 0; y < accessor.Height; y++)
                    {
                        var pixelRow = accessor.GetRowSpan(y);

                        for (int x = 0; x < pixelRow.Length; x++)
                        {
                            ref Rgba32 pixel = ref pixelRow[x];
                            (pixel.B, pixel.R) = (pixel.R, pixel.B);
                        }
                    }
                });

                var pngData = new MemoryStream(SourceArt.SizeOnDisk);
                var encoder = new PngEncoder() { ChunkFilter = PngChunkFilter.ExcludeAll };
                decodedImageData.SaveAsPng(pngData, encoder);

                SourceArt.Data = pngData.ToArray();
                SourceArt.NumElements = SourceArt.Data.Length;
                SourceArt.SizeOnDisk = SourceArt.NumElements;

                // Make sure the texture has the bIsSourceArtUncompressed property set to true
                // EnsureSourceArtUncompressedPropertyIsSet();
            }
        }

        protected void EnsureSourceArtUncompressedPropertyIsSet()
        {
            var existingProp = GetSerializedProperty("bIsSourceArtUncompressed") as USerializedBoolProperty;

            if (existingProp != null)
            {
                if (existingProp.Tag != null)
                {
                    var tag = existingProp.Tag.Value;
                    tag.BoolVal = true;
                    existingProp.Tag = tag;
                }

                existingProp.Value = true;

                return;
            }

            var newTag = new FPropertyTag()
            {
                Name = Archive.GetOrCreateName("bIsSourceArtUncompressed"),
                Type = Archive.GetOrCreateName("BoolProperty"),
                Size = 0,
                ArrayIndex = 0,
                BoolVal = true
            };

            var newProp = new USerializedBoolProperty(Archive, /* backingProperty */ null, newTag);

            SerializedProperties.Add(newProp);
        }

        protected int FindBestMipsIndex()
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
    
        protected CompressionFormat GetCompressionFormat()
        {
            var formatProperty = GetSerializedProperty("Format") as USerializedByteProperty;

            if (formatProperty == null || formatProperty.EnumValue == null)
            {
                throw new Exception("Can't determine the compression format for this texture");
            }

            var compressionNoAlphaProperty = GetSerializedProperty("CompressionNoAlpha") as USerializedBoolProperty;
            bool bCompressionNoAlpha = compressionNoAlphaProperty?.BoolValue ?? false;

            string format = formatProperty.EnumValue;
            switch (format)
            {
                case "PF_DXT1":
                    return bCompressionNoAlpha ? CompressionFormat.Bc1 : CompressionFormat.Bc1WithAlpha;
                case "PF_DXT3":
                    return CompressionFormat.Bc2;
                case "PF_DXT5":
                    return CompressionFormat.Bc3;
                case "PF_BC5":
                    return CompressionFormat.Bc5;
                case "PF_V8U8":
                    return CompressionFormat.Rg;
                case "PF_A8R8G8B8":
                    return CompressionFormat.Rgba;
                case "PF_G8":
                    return CompressionFormat.R;
                default:
                    throw new Exception($"Unsupported EPixelFormat value {formatProperty.EnumValue}");
            }
        }
    
        /// <summary>
        /// Loads the texture data from a file (if needed) and decompresses it (if needed), storing the raw data. Note that this undoes
        /// the compression of the <see cref="FUntypedBulkData"/>, but does not remove any compression inherent to the pixel format 
        /// (e.g. DXT1).
        /// </summary>
        /// <returns>A new <see cref="FUntypedBulkData"/>; the original is unmodified.</returns>
        protected FUntypedBulkData LoadAndDecompressTextureData(string? tfcName, CompressionFormat compressionFormat, int sizeX, int sizeY, FUntypedBulkData inData)
        {
            FUntypedBulkData outData;
            bool storeInSeparateFile = inData.BulkDataFlags.HasFlag(EBulkDataFlags.StoreInSeparateFile);

            // Check if there's any work to do first
            if (!storeInSeparateFile && !inData.IsCompressed)
            {
                return inData;
            }

            outData = new FUntypedBulkData();
            outData.CloneFromOtherArchive(inData, null, null);

            if (inData.NumElements == 0)
            {
                return outData;
            }

            if (storeInSeparateFile)
            {
#if DEBUG
                if (tfcName == null)
                {
                    throw new Exception($"UTexture2D {FullObjectPath} doesn't have a TextureFileCacheName property!");
                }
#endif

                outData.SetData(Archive.ParentLinker.ReadTextureData(tfcName, inData.Offset, inData.SizeOnDisk), outData.NumElements);
            }

            outData.Decompress();

            /*
            if (compressionFormat != CompressionFormat.Rgba) 
            {
                var decoder = new BcDecoder();
                var decodedImageData = decoder.DecodeRawToImageRgba32(outData.Data, sizeX, sizeY, compressionFormat);
                byte[] rawData = new byte[sizeX * sizeY * 4];
                decodedImageData.CopyPixelDataTo(rawData);
                outData.SetData(rawData, outData.NumElements);
            }
            */

            return outData;
        }
    }
}
