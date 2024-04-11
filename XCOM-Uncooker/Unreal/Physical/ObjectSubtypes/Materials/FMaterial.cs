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
            stream.Int32(out TexCoordIndex);
            stream.Int32(out TextureIndex);
            stream.Float32(out UScale);
            stream.Float32(out VScale);
        }
    }

    public class FMaterial : IUnrealSerializable
    {
        #region Serialized data

        public string[] CompileErrors;

        // Key is the index of a material expression; value is the "texture dependency length" for that expression
        [Index(typeof(UObject))]
        public IDictionary<int, int> TextureDependencyLengthMap;

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
            stream.StringArray(out CompileErrors);
            stream.Map(out TextureDependencyLengthMap);
            stream.Int32(out MaxTextureDependencyLength);
            stream.Guid(out Id);
            stream.UInt32(out NumUserTexCoords);
            stream.Int32Array(out UniformExpressionTextures);
            stream.BoolAsInt32(out bUsesSceneColor);
            stream.BoolAsInt32(out bUsesSceneDepth);
            stream.BoolAsInt32(out bUsesDynamicParameter);
            stream.BoolAsInt32(out bUsesLightmapUVs);
            stream.BoolAsInt32(out bUsesMaterialVertexPositionOffset);
            stream.Enum32(out UsingTransforms);
            stream.Array(out TextureLookups);
            stream.Enum32(out DroppedFallbackComponents);
        }
    }
}
