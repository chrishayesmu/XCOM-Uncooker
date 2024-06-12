using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical;

namespace XCOM_Uncooker.Unreal.Shaders
{
    public struct FShader : IUnrealSerializable
    {
        public FShader() { }

        #region Serialized data

        public byte TargetPlatform;
        
        public byte TargetFrequency;

        public byte[] Code;

        public uint ParameterMapCRC;
        
        public uint Unknown;

        public Guid Id;

        public FShaderType Type;

        public byte[] Hash = new byte[20]; // fixed size

        public uint NumInstructions;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.UInt8(ref TargetPlatform);
            stream.UInt8(ref TargetFrequency);
            stream.ByteArray(ref Code);

            if (stream.IsRead && stream.Archive.FileName.StartsWith("RefShaderCache"))
            {
                stream.UInt32(ref Unknown);
            }

            stream.UInt32(ref ParameterMapCRC);
            stream.Guid(ref Id);
            stream.Object(ref Type);
            stream.Bytes(ref Hash, 20);
            stream.UInt32(ref NumInstructions);
        }
    }

    // This is FShader when serialized with operator<<
    public struct FShaderMetadata : IUnrealSerializable
    {
        #region Serialized data

        public Guid ShaderId;

        public FShaderType ShaderType;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Guid(ref ShaderId);
            stream.Object(ref ShaderType);
        }
    }

    public struct FShaderType : IUnrealSerializable
    {
        #region Serialized data

        // In a global shader cache (.bin) file, names are serialized as strings. In local shader cache (.upk)
        // files, they follow normal name serialization.
        public string NameAsString;

        public FName Name;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FShaderType) sourceObj;

            Name = other.Name is not null ? destArchive.MapNameFromSourceArchive(other.Name) : destArchive.GetOrCreateName(other.NameAsString);
            NameAsString = Name.ToString();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            if (stream.Archive.FileName.StartsWith("GlobalShaderCache"))
            {
                stream.String(ref NameAsString);
            }
            else
            {
                stream.Name(ref Name);
            }
        }
    }

    public struct FShaderPtr : IUnrealSerializable
    {
        #region Serialized data

        public Guid ShaderId;

        public FShaderType ShaderType;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FShaderPtr) sourceObj;

            ShaderId = other.ShaderId;

            ShaderType = new FShaderType();
            ShaderType.CloneFromOtherArchive(other.ShaderType, sourceArchive, destArchive);
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Guid(ref ShaderId);
            stream.Object(ref ShaderType);
        }
    }

    public struct FShaderCacheEntry : IUnrealSerializable
    {
        public FShaderCacheEntry() { }

        #region Serialized data

        public FShaderType ShaderType;

        public Guid ShaderId;

        public byte[] SavedHash = new byte[20]; // fixed size

        public int SkipOffset; // offset of the following shader

        public ushort[] Serializations;

        public FShader Shader;

        public byte[] ShaderData;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FShaderCacheEntry) sourceObj;

            ShaderType = new FShaderType();
            ShaderType.CloneFromOtherArchive(other.ShaderType, sourceArchive, destArchive);

            ShaderId = other.ShaderId;
            SavedHash = other.SavedHash;
            SkipOffset = other.SkipOffset;

            // Copy the serializations, but skip index 4 because it's for a value we aren't serializing out
            Serializations = new ushort[other.Serializations.Length - 1];
            Serializations[0] = other.Serializations[0];
            Serializations[1] = other.Serializations[1];
            Serializations[2] = other.Serializations[2];
            Serializations[3] = other.Serializations[3];

            for (int i = 4; i < Serializations.Length; i++)
            {
                Serializations[i] = other.Serializations[i + 1];
            }

            Shader = new FShader();
            Shader.CloneFromOtherArchive(other.Shader, sourceArchive, destArchive);

            ShaderData = other.ShaderData;
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref ShaderType);
            stream.Guid(ref ShaderId);
            stream.Bytes(ref SavedHash, 20);

            long skipOffsetPosition = stream.Position;
            stream.Int32(ref SkipOffset);

            if (stream.IsRead)
            {
                stream.UInt16Array(ref Serializations);
            }
            else
            {
                // We need to skip one index in this array when writing out
                int newLength = Serializations.Length - 1;
                stream.Int32(ref newLength);

                for (int i = 0; i < Serializations.Length; i++)
                {
                    if (i == 4)
                    {
                        continue;
                    }

                    stream.UInt16(ref Serializations[i]);
                }
            }

            stream.Object(ref Shader);

            if (stream.IsRead)
            {
                int remainingBytes = SkipOffset - (int) stream.Position;
                stream.Bytes(ref ShaderData, remainingBytes);
            }
            else
            {
                stream.Bytes(ref ShaderData, ShaderData.Length);

                // Go back and overwrite the skipOffset
                int endPosition = (int) stream.Position;

                stream.Seek(skipOffsetPosition, SeekOrigin.Begin);
                stream.Int32(ref endPosition);
                stream.Seek(endPosition, SeekOrigin.Begin);
            }
        }
    }

    public class FShaderCache : IUnrealSerializable
    {
        #region Serialized data

        public byte Platform;

        public uint DummyCompressedCache; // would be a map size, but the map is never populated

        public FShaderCacheEntry[] CacheEntries;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FShaderCache) sourceObj;

            Platform = other.Platform;
            DummyCompressedCache = other.DummyCompressedCache;
            CacheEntries = new FShaderCacheEntry[other.CacheEntries.Length];

            for (int i = 0; i < CacheEntries.Length; i++)
            {
                CacheEntries[i] = new FShaderCacheEntry();
                CacheEntries[i].CloneFromOtherArchive(other.CacheEntries[i], sourceArchive, destArchive);
            }
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.UInt8(ref Platform);
            stream.UInt32(ref DummyCompressedCache);

#if DEBUG
            if (DummyCompressedCache != 0)
            {
                Debugger.Break();
            }
#endif

            stream.Array(ref CacheEntries);
        }
    }
}
