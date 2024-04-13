using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Materials
{
    [Flags]
    public enum ECoordTransformUsage
    {
        None = 0x0,
        World = 0x1,
        View = 0x2,
        Local = 0x4,
        WorldPos = 0x8
    };

    [Flags]
    public enum EDroppedFallbackComponents : uint
    {
        None = 0,
        Specular = 0x1,
        Normal = 0x2,
        Diffuse = 0x4,
        Emissive = 0x8,
        Failed = 0x80000000
    };

    public struct FTextureLookup : IUnrealSerializable
    {
        public int TexCoordIndex;
        public int TextureIndex;
        public float UScale;
        public float VScale;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref TexCoordIndex);
            stream.Int32(ref TextureIndex);
            stream.Float32(ref UScale);
            stream.Float32(ref VScale);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FTextureLookup) sourceObj;

            TexCoordIndex = other.TexCoordIndex;
            TextureIndex = other.TextureIndex;
            UScale = other.UScale;
            VScale = other.VScale;
        }
    }

    public class FMaterial : IUnrealSerializable
    {
        #region Serialized data

        public string[] CompileErrors;

        // Key is the index of a material expression; value is the "texture dependency length" for that expression
        [Index(typeof(UObject))]
        public IDictionary<int, int> TextureDependencyLengthMap = new Dictionary<int, int>();

        public int MaxTextureDependencyLength;

        public Guid Id;

        public uint NumUserTexCoords;

        [Index(typeof(UObject))]
        public int[] UniformExpressionTextures;

        public bool bUsesSceneColor;
        public bool bUsesSceneDepth;
        public bool bUsesDynamicParameter;
        public bool bUsesLightmapUVs;
        public bool bUsesMaterialVertexPositionOffset;

        public ECoordTransformUsage UsingTransforms;

        public FTextureLookup[] TextureLookups;

        public EDroppedFallbackComponents DroppedFallbackComponents;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.StringArray(ref CompileErrors);
            stream.Map(ref TextureDependencyLengthMap);
            stream.Int32(ref MaxTextureDependencyLength);
            stream.Guid(ref Id);
            stream.UInt32(ref NumUserTexCoords);
            stream.Int32Array(ref UniformExpressionTextures);
            stream.BoolAsInt32(ref bUsesSceneColor);
            stream.BoolAsInt32(ref bUsesSceneDepth);
            stream.BoolAsInt32(ref bUsesDynamicParameter);
            stream.BoolAsInt32(ref bUsesLightmapUVs);
            stream.BoolAsInt32(ref bUsesMaterialVertexPositionOffset);
            stream.Enum32(ref UsingTransforms);
            stream.Array(ref TextureLookups);
            stream.Enum32(ref DroppedFallbackComponents);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FMaterial) sourceObj;

            CompileErrors = other.CompileErrors;

            foreach (var entry in other.TextureDependencyLengthMap)
            {
                int newKey = destArchive.MapIndexFromSourceArchive(entry.Key, sourceArchive);
                TextureDependencyLengthMap.Add(newKey, entry.Value);
            }

            MaxTextureDependencyLength = other.MaxTextureDependencyLength;
            Id = other.Id;
            NumUserTexCoords = other.NumUserTexCoords;
            UniformExpressionTextures = destArchive.MapIndicesFromSourceArchive(other.UniformExpressionTextures, sourceArchive);
            bUsesSceneColor = other.bUsesSceneColor;
            bUsesSceneDepth = other.bUsesSceneDepth;
            bUsesDynamicParameter = other.bUsesDynamicParameter;
            bUsesLightmapUVs = other.bUsesLightmapUVs;
            bUsesMaterialVertexPositionOffset = other.bUsesMaterialVertexPositionOffset;
            UsingTransforms = other.UsingTransforms;
            TextureLookups = other.TextureLookups;
            DroppedFallbackComponents = other.DroppedFallbackComponents;
        }
    }
}
