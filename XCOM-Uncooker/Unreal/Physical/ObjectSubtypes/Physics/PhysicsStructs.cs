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
            stream.Object(ref Origin);
            stream.Object(ref BoxExtent);
            stream.Float32(ref SphereRadius);
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
            stream.Bytes(ref Data, 64);
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
            stream.Int32(ref Material);
            stream.UInt32(ref PolyFlags);
            stream.Int32(ref pBase);
            stream.Int32(ref vNormal);
            stream.Int32(ref vTextureU);
            stream.Int32(ref vTextureV);
            stream.Int32(ref iBrushPoly);
            stream.Int32(ref Actor);
            stream.Object(ref Plane);
            stream.Float32(ref ShadowMapScale);
            stream.UInt32(ref LightingChannels);
            stream.Int32(ref iLightmassIndex);
        }
    }

    public struct FKCachedConvexDataElement : IUnrealSerializable
    {
        #region Serialized data

        public byte[] Data;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.BulkArray(ref Data);
        }
    }

    public struct FKCachedConvexData : IUnrealSerializable
    {
        #region Serialized data

        public FKCachedConvexDataElement[] CachedConvexElements;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Array(ref CachedConvexElements);
        }
    }

    public struct FKCachedPerTriData : IUnrealSerializable
    {
        #region Serialized data

        public byte[] CachedPerTriData;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.BulkArray(ref CachedPerTriData);
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
            stream.BoolAsInt32(ref bUseTwoSidedLighting);
            stream.BoolAsInt32(ref bShadowIndirectOnly);
            stream.Float32(ref FullyOccludedSamplesFraction);
            stream.BoolAsInt32(ref bUseEmissiveForStaticLighting);
            stream.Float32(ref EmissiveLightFalloffExponent);
            stream.Float32(ref EmissiveLightExplicitInfluenceRadius);
            stream.Float32(ref EmissiveBoost);
            stream.Float32(ref DiffuseBoost);
            stream.Float32(ref SpecularBoost);
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
            stream.Object(ref Position);
            stream.Object(ref TangentX);
            stream.Object(ref TangentZ);
            stream.Object(ref TexCoord);
            stream.Object(ref ShadowTexCoord);
        }
    }

    public struct FModelVertexBuffer : IUnrealSerializable
    {
        #region Serialized data

        public FModelVertex[] Vertices;

        #endregion
     
        public void Serialize(IUnrealDataStream stream)
        {
            stream.BulkArray(ref Vertices, 36);
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
            stream.Int32(ref pVertex);
            stream.Int32(ref iSide);
            stream.Object(ref ShadowTexCoord);
            stream.Object(ref BackfaceShadowTexCoord);
        }
    }
}
