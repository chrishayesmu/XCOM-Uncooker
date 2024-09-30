using SixLabors.ImageSharp.Textures.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Materials;

namespace UnrealArchiveLibrary.Unreal.Shaders
{
    public struct FMaterialShaderMap : IUnrealSerializable
    {
        #region Serialized data

        public FStaticParameterSet StaticParameters;

        public int ShaderMapVersion;

        public int ShaderMapLicenseeVersion;

        public int SkipOffset;

        public byte[] ShaderData;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            throw new NotImplementedException();
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
            throw new NotImplementedException();
        }
    }

    public class UShaderCache(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public int ShaderCachePriority;

        public FShaderCache ShaderCache;

        public FMaterialShaderMap[] MaterialShaderMaps;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(ref ShaderCachePriority);
            stream.Object(ref ShaderCache);
            stream.Array(ref MaterialShaderMaps);
        }
    }
}
