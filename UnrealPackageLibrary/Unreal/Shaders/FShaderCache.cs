using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal;

namespace UnrealArchiveLibrary.Unreal.Shaders
{
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

            Name = destArchive.GetOrCreateName(other.NameAsString);
        }

        public void Serialize(IUnrealDataStream stream)
        {
            if (stream.IsRead)
            {
                stream.String(ref NameAsString);
            }
            else
            {
                stream.Name(ref Name);
            }
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
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

        public void PopulateDependencies(List<int> dependencyIndices)
        {
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
            ShaderData = other.ShaderData;
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref ShaderType);
            stream.Guid(ref ShaderId);
            stream.Bytes(ref SavedHash, 20);
            stream.Int32(ref SkipOffset);

            if (stream.IsRead)
            {
                int remainingBytes = SkipOffset - (int) stream.Position;
                stream.Bytes(ref ShaderData, remainingBytes);
            }
            else
            {
                stream.Bytes(ref ShaderData, ShaderData.Length);
            }
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }

    public class FShaderCache : IUnrealSerializable
    {
        #region Serialized data

        public byte Platform;

        public uint Dummy; // would be a map size, but the map is never populated

        public FShaderCacheEntry[] CacheEntries;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FShaderCache) sourceObj;

            Platform = other.Platform;
            Dummy = other.Dummy;
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
            stream.UInt32(ref Dummy);

#if DEBUG
            if (Dummy != 0)
            {
                Debugger.Break();
            }
#endif

            stream.Array(ref CacheEntries);
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }
}
