using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Materials;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Physics
{
    [FixedSize(28)]
    public struct FBoxSphereBounds : IUnrealSerializable
    {
        #region Serialized data

        public FVector Origin;
        
        public FVector BoxExtent;

        public float SphereRadius;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(out Origin);
            stream.Object(out BoxExtent);
            stream.Float32(out SphereRadius);
        }
    }

    [FixedSize(64)]
    public struct FBspNode : IUnrealSerializable
    {
        #region Serialized data

        public byte[] Data;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Bytes(out Data, 64);
        }
    }

    [FixedSize(60)]
    public struct FBspSurf : IUnrealSerializable
    {
        #region Serialized data

        [Index(typeof(UMaterial))]
        public int Material;

        public uint PolyFlags;

        public int pBase;

        public int vNormal;

        public int vTextureU;

        public int vTextureV;

        public int iBrushPoly;

        public int Actor;

        public FPlane Plane;

        public float ShadowMapScale;

        public uint LightingChannels;

        public int iLightmassIndex;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(out Material);
            stream.UInt32(out PolyFlags);
            stream.Int32(out pBase);
            stream.Int32(out vNormal);
            stream.Int32(out vTextureU);
            stream.Int32(out vTextureV);
            stream.Int32(out iBrushPoly);
            stream.Int32(out Actor);
            stream.Object(out Plane);
            stream.Float32(out ShadowMapScale);
            stream.UInt32(out LightingChannels);
            stream.Int32(out iLightmassIndex);
        }
    }

    public struct FKCachedConvexDataElement : IUnrealSerializable
    {
        #region Serialized data

        public byte[] Data;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.BulkArray(out Data);
        }
    }

    public struct FKCachedConvexData : IUnrealSerializable
    {
        #region Serialized data

        public FKCachedConvexDataElement[] CachedConvexElements;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Array(out CachedConvexElements);
        }
    }

    public struct FKCachedPerTriData : IUnrealSerializable
    {
        #region Serialized data

        public byte[] CachedPerTriData;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.BulkArray(out CachedPerTriData);
        }
    }

    [FixedSize(36)]
    public struct FLightmassPrimitiveSettings : IUnrealSerializable
    {
        #region Serialized data

        public bool bUseTwoSidedLighting; // UBOOL

        public bool bShadowIndirectOnly; // UBOOL

        public float FullyOccludedSamplesFraction;

        public bool bUseEmissiveForStaticLighting; // UBOOL

        public float EmissiveLightFalloffExponent;

        public float EmissiveLightExplicitInfluenceRadius;

        public float EmissiveBoost;

        public float DiffuseBoost;

        public float SpecularBoost;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.BoolAsInt32(out bUseTwoSidedLighting);
            stream.BoolAsInt32(out bShadowIndirectOnly);
            stream.Float32(out FullyOccludedSamplesFraction);
            stream.BoolAsInt32(out bUseEmissiveForStaticLighting);
            stream.Float32(out EmissiveLightFalloffExponent);
            stream.Float32(out EmissiveLightExplicitInfluenceRadius);
            stream.Float32(out EmissiveBoost);
            stream.Float32(out DiffuseBoost);
            stream.Float32(out SpecularBoost);
        }
    }

    [FixedSize(36)]
    public struct FModelVertex : IUnrealSerializable
    {
        #region Serialized data

        public FVector Position;

        public FPackedNormal TangentX;

        public FPackedNormal TangentZ;	

        public FVector2D TexCoord;

        public FVector2D ShadowTexCoord;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(out Position);
            stream.Object(out TangentX);
            stream.Object(out TangentZ);
            stream.Object(out TexCoord);
            stream.Object(out ShadowTexCoord);
        }
    }

    public struct FModelVertexBuffer : IUnrealSerializable
    {
        #region Serialized data

        public FModelVertex[] Vertices;

        #endregion
     
        public void Serialize(IUnrealDataStream stream)
        {
            stream.BulkArray(out Vertices, 36);
        }
    }

    [FixedSize(24)]
    public struct FVert : IUnrealSerializable
    {
        #region Serialized data

        public int pVertex;

        public int iSide;

        public FVector2D ShadowTexCoord;

        public FVector2D BackfaceShadowTexCoord;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(out pVertex);
            stream.Int32(out iSide);
            stream.Object(out ShadowTexCoord);
            stream.Object(out BackfaceShadowTexCoord);
        }
    }
}
