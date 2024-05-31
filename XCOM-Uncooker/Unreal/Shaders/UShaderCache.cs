﻿using SixLabors.ImageSharp.Textures.PixelFormats;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Materials;

namespace XCOM_Uncooker.Unreal.Shaders
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