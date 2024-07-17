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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FBoxSphereBounds) sourceObj;

            Origin = other.Origin;
            BoxExtent = other.BoxExtent;
            SphereRadius = other.SphereRadius;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FBspNode) sourceObj;

            Data = other.Data;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
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

        [Index(typeof(UObject))]
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FBspSurf) sourceObj;

            Material = destArchive.MapIndexFromSourceArchive(other.Material, sourceArchive);
            PolyFlags = other.PolyFlags;
            pBase = other.pBase;
            vNormal = other.vNormal;
            vTextureU = other.vTextureU;
            vTextureV = other.vTextureV;
            iBrushPoly = other.iBrushPoly;
            Actor = destArchive.MapIndexFromSourceArchive(other.Actor, sourceArchive);
            Plane = other.Plane;
            ShadowMapScale = other.ShadowMapScale;
            LightingChannels = other.LightingChannels;
            iLightmassIndex = other.iLightmassIndex;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
            dependencyIndices.Add(Material);
            dependencyIndices.Add(Actor);
        }
    }

    public struct FKCachedConvexDataElement : IUnrealSerializable
    {
        #region Serialized data

        public FByteArrayWithSize Data;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref Data);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FKCachedConvexDataElement) sourceObj;

            Data = other.Data;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FKCachedConvexData) sourceObj;

            CachedConvexElements = other.CachedConvexElements;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }

    public struct FKCachedPerTriData : IUnrealSerializable
    {
        #region Serialized data

        public FByteArrayWithSize CachedPerTriData;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref CachedPerTriData);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FKCachedPerTriData) sourceObj;

            CachedPerTriData = other.CachedPerTriData;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FLightmassPrimitiveSettings) sourceObj;

            bUseTwoSidedLighting = other.bUseTwoSidedLighting;
            bShadowIndirectOnly = other.bShadowIndirectOnly;
            FullyOccludedSamplesFraction = other.FullyOccludedSamplesFraction;
            bUseEmissiveForStaticLighting = other.bUseEmissiveForStaticLighting;
            EmissiveLightFalloffExponent = other.EmissiveLightFalloffExponent;
            EmissiveLightExplicitInfluenceRadius = other.EmissiveLightExplicitInfluenceRadius;
            EmissiveBoost = other.EmissiveBoost;
            DiffuseBoost = other.DiffuseBoost;
            SpecularBoost = other.SpecularBoost;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FModelVertex) sourceObj;

            Position = other.Position;
            TangentX = other.TangentX;
            TangentZ = other.TangentZ;
            TexCoord = other.TexCoord;
            ShadowTexCoord = other.ShadowTexCoord;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FModelVertexBuffer) sourceObj;

            Vertices = other.Vertices;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FVert) sourceObj;

            pVertex = other.pVertex;
            iSide = other.iSide;
            ShadowTexCoord = other.ShadowTexCoord;
            BackfaceShadowTexCoord = other.BackfaceShadowTexCoord;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }
}
