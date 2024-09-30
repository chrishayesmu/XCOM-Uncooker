using lzo.net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnrealArchiveLibrary.IO
{
    public enum ECompressionMethod
    {
        None,
        ZLIB,
        LZO,
        LZX
    }

    public abstract class IOUtils
    {
        private const int MaxBufferSize = 8192;

        public static void Decompress(byte[] compressedData, int uncompressedSize, byte[] destBuffer, ref int destOffset, ECompressionMethod compressionMethod)
        {
            if (uncompressedSize == 0)
            {
                return;
            }

            switch (compressionMethod)
            {
                case ECompressionMethod.LZO:
                    DecompressLZO(compressedData, uncompressedSize, destBuffer, ref destOffset);
                    break;
                default:
                    throw new ArgumentException($"Unsupported compression method {compressionMethod}");
            }
        }

        public static void DecompressLZO(byte[] data, int uncompressedSize, byte[] destBuffer, ref int destOffset)
        {
#if false
            var iron = new Iron();
            
            using (IronCompressResult uncompressedData = iron.Decompress(Codec.LZO, data, uncompressedSize))
            {
                var destSpan = new Span<byte>(destBuffer, destOffset, uncompressedSize);
                uncompressedData.AsSpan().CopyTo(destSpan);
            }
#else
            var dataStream = new MemoryStream(data);
            var lzoStream = new LzoStream(dataStream, CompressionMode.Decompress);

            int totalRead = 0;

            while (uncompressedSize > 0)
            {
                // This returns the number of uncompressed bytes read
                int read = lzoStream.Read(destBuffer, destOffset, uncompressedSize);

                // The compressed data can terminate before filling the entire allocated chunk space
                if (read <= 0)
                {
                    break;
                }

                totalRead += read;
                destOffset += read;
                uncompressedSize -= read;
            }
#endif
        }
    }
}
