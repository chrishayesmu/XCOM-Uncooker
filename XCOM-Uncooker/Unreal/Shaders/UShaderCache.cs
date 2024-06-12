using SixLabors.ImageSharp.Textures.PixelFormats;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Materials;

namespace XCOM_Uncooker.Unreal.Shaders
{
    public struct FMeshMaterialShaderMap : IUnrealSerializable
    {
        #region Serialized data

        public IDictionary<FShaderType, FShaderMetadata> Shaders;

        public FName VertexFactoryType;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Map(ref Shaders);
            stream.Name(ref VertexFactoryType);
        }
    }

    public struct FMaterialShaderMap : IUnrealSerializable
    {
        #region Serialized data

        public IDictionary<FShaderType, FShaderMetadata> Shaders;

        public FMeshMaterialShaderMap[] MeshShaderMaps;

        public Guid MaterialId;

        public string FriendlyName;

        public FStaticParameterSet StaticParameters;

        public FUniformExpressionSet UniformExpressionSet;

        public uint Platform;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Map(ref Shaders);
            stream.Array(ref MeshShaderMaps);
            stream.Guid(ref MaterialId);
            stream.String(ref FriendlyName);
            stream.Object(ref StaticParameters);
            stream.Object(ref UniformExpressionSet);
            stream.UInt32(ref Platform);
        }

        public bool ContainsXComParameterTypes()
        {
            var allExpressions = FlattenMaterialUniformExpressions();

            return allExpressions.Any(exp => exp.TypeName == "FMaterialUniformExpressionXComVectorParameter");
        }

        private List<FMaterialUniformExpression> FlattenMaterialUniformExpressions()
        {
            var flattened = new List<FMaterialUniformExpression>();

            flattened.AddRange(UniformExpressionSet.PixelExpressions.UniformVectorExpressions);
            flattened.AddRange(UniformExpressionSet.PixelExpressions.UniformScalarExpressions);
            flattened.AddRange(UniformExpressionSet.UniformCubeTextureExpressions);
            flattened.AddRange(UniformExpressionSet.PixelExpressions.Uniform2DTextureExpressions);
            flattened.AddRange(UniformExpressionSet.VertexExpressions.UniformVectorExpressions);
            flattened.AddRange(UniformExpressionSet.VertexExpressions.UniformScalarExpressions);
            flattened.AddRange(UniformExpressionSet.VertexExpressions.Uniform2DTextureExpressions);
            flattened.AddRange(UniformExpressionSet.HullExpressions.UniformVectorExpressions);
            flattened.AddRange(UniformExpressionSet.HullExpressions.UniformScalarExpressions);
            flattened.AddRange(UniformExpressionSet.HullExpressions.Uniform2DTextureExpressions);
            flattened.AddRange(UniformExpressionSet.DomainExpressions.UniformVectorExpressions);
            flattened.AddRange(UniformExpressionSet.DomainExpressions.UniformScalarExpressions);
            flattened.AddRange(UniformExpressionSet.DomainExpressions.Uniform2DTextureExpressions);

            var expressionsToProcess = new Stack<FMaterialUniformExpression>(flattened);

            while (expressionsToProcess.TryPop(out var expression))
            {
                FMaterialUniformExpression[] subexpressions = expression.Expression switch
                {
                    FMaterialUniformExpressionAppendVector av => [av.A, av.B],
                    FMaterialUniformExpressionClamp clamp => [clamp.Input, clamp.Min, clamp.Max],
                    FMaterialUniformExpressionFoldedMath math => [math.A, math.B],
                    FMaterialUniformExpressionSine sine => [sine.X],
                    FMaterialUniformExpression_SingleExpressionInput input => [input.A],
                    FMaterialUniformExpression_DoubleExpressionInput input => [input.A, input.B],
                    _ => []
                };

                flattened.AddRange(subexpressions);

                foreach (var subexp in subexpressions)
                {
                    expressionsToProcess.Push(subexp);
                }
            }

            return flattened;
        }
    }

    public struct FMaterialShaderMapContainer : IUnrealSerializable
    {
        #region Serialized data

        public FStaticParameterSet StaticParameters;

        public int ShaderMapVersion;

        public int ShaderMapLicenseeVersion;

        public int SkipOffset;

        public FMaterialShaderMap ShaderMap;

        public byte[] LeftoverData;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            throw new NotImplementedException();
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref StaticParameters);
            stream.Int32(ref ShaderMapVersion);
            stream.Int32(ref ShaderMapLicenseeVersion);

            int skipOffsetPosition = (int) stream.Position;
            stream.Int32(ref SkipOffset);
            stream.Object(ref ShaderMap);

            if (stream.IsRead)
            {
                // Every single one of these has 16 bytes of unknown data here, and they're always all 0. No idea why but
                // they only exist in XCOM's shader cache and not one from the UDK
                int numBytes = SkipOffset - (int) stream.Position;
                stream.Bytes(ref LeftoverData, numBytes);
            }

            if (stream.IsWrite)
            {
                int shaderMapEndPosition = (int) stream.Position;

                stream.Seek(skipOffsetPosition, SeekOrigin.Begin);
                stream.Int32(ref shaderMapEndPosition);
                stream.Seek(shaderMapEndPosition, SeekOrigin.Begin);
            }
        }
    }

    public class UShaderCache(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public int ShaderCachePriority;

        public FShaderCache ShaderCache;

        public FMaterialShaderMapContainer[] MaterialShaderMaps;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(ref ShaderCachePriority);
            stream.Object(ref ShaderCache);

            if (stream.IsRead)
            {
                stream.Array(ref MaterialShaderMaps);
            }
            else
            {
                var filteredShaderMaps = MaterialShaderMaps.Where(m => !m.ShaderMap.ContainsXComParameterTypes()).ToArray();
                stream.Array(ref filteredShaderMaps);
            }
        }
    }
}
