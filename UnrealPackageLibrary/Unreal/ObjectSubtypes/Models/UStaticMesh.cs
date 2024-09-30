using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Actor;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Components;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Physics;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.Models
{
    public struct FFragmentRange : IUnrealSerializable
    {
        #region Serialized data

        public int BaseIndex;

        public int NumPrimitives;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref BaseIndex);
            stream.Int32(ref NumPrimitives);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FFragmentRange) sourceObj;

            BaseIndex = other.BaseIndex;
            NumPrimitives = other.NumPrimitives;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }

    public struct FPositionVertexBuffer : IUnrealSerializable
    {
        #region Serialized data

        public uint Stride;

        public uint NumVertices;

        public FByteArrayWithSize VertexData;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.UInt32(ref Stride);
            stream.UInt32(ref NumVertices);
            stream.Object(ref VertexData);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FPositionVertexBuffer) sourceObj;

            Stride = other.Stride;
            NumVertices = other.NumVertices;
            VertexData = other.VertexData;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }

    public struct FStaticMeshElement : IUnrealSerializable
    {
        #region Serialized data

        [Index(typeof(UObject))]
        public int Material;

        public bool EnableCollision; // UBOOL

        public bool OldEnableCollision; // UBOOL

        public bool bEnableShadowCasting; // UBOOL

        public uint FirstIndex;

        public uint NumTriangles;

        public uint MinVertexIndex;

        public uint MaxVertexIndex;

        public int MaterialIndex;

        public FFragmentRange[] Fragments;

        public bool LoadPlatformData;

        // public FPS3StaticMeshData PlatformData; // LoadPlatformData doesn't seem to ever be true in XCOM,
                                                   // so we don't actually need this field

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref Material);
            stream.BoolAsInt32(ref EnableCollision);
            stream.BoolAsInt32(ref OldEnableCollision);
            stream.BoolAsInt32(ref bEnableShadowCasting);
            stream.UInt32(ref FirstIndex);
            stream.UInt32(ref NumTriangles);
            stream.UInt32(ref MinVertexIndex);
            stream.UInt32(ref MaxVertexIndex);
            stream.Int32(ref MaterialIndex);
            stream.Array(ref Fragments);
            stream.Bool(ref LoadPlatformData);

#if DEBUG
            if (LoadPlatformData)
            {
                Debugger.Break();
            }
#endif
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FStaticMeshElement) sourceObj;

            Material = destArchive.MapIndexFromSourceArchive(other.Material, sourceArchive);
            EnableCollision = other.EnableCollision;
            OldEnableCollision = other.OldEnableCollision;
            bEnableShadowCasting = other.bEnableShadowCasting;
            FirstIndex = other.FirstIndex;
            NumTriangles = other.NumTriangles;
            MinVertexIndex = other.MinVertexIndex;
            MaxVertexIndex = other.MaxVertexIndex;
            MaterialIndex = other.MaterialIndex;
            Fragments = other.Fragments;
            LoadPlatformData = other.LoadPlatformData;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
            dependencyIndices.Add(Material);
        }
    }

    public struct FStaticMeshLODInfo : IUnrealSerializable
    {
        public void Serialize(IUnrealDataStream stream)
        {
            // Deliberate no-op; it seems like this is only "serialized" during internal operations, and
            // not to an archive file. Leaving this here to show that it's not an accidental omission.
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }

    public struct FStaticMeshOptimizationSettings : IUnrealSerializable
    {
        #region Serialized data

        public float MaxDeviationPercentage;

        public byte SilhouetteImportance;
        
        public byte TextureImportance;

        public byte NormalMode;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Float32(ref MaxDeviationPercentage);
            stream.UInt8(ref SilhouetteImportance);
            stream.UInt8(ref TextureImportance);
            stream.UInt8(ref NormalMode);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FStaticMeshOptimizationSettings) sourceObj;

            MaxDeviationPercentage = other.MaxDeviationPercentage;
            SilhouetteImportance = other.SilhouetteImportance;
            TextureImportance = other.TextureImportance;
            NormalMode = other.NormalMode;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }

    public struct FStaticMeshRenderData : IUnrealSerializable
    {
        public FStaticMeshRenderData() {}

        #region Serialized data

        public FUntypedBulkData RawTriangles = new FUntypedBulkData();

        public FStaticMeshElement[] Elements;

        public FPositionVertexBuffer PositionVertexBuffer;

        public FStaticMeshVertexBuffer VertexBuffer;

        public FColorVertexBuffer ColorVertexBuffer;

        public uint NumVertices;

        public FByteArrayWithSize IndexBuffer;

        public FByteArrayWithSize WireframeIndexBuffer;

        public FByteArrayWithSize AdjacencyIndexBuffer;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            RawTriangles.Serialize(stream);
            stream.Array(ref Elements);
            stream.Object(ref PositionVertexBuffer);
            stream.Object(ref VertexBuffer);
            stream.Object(ref ColorVertexBuffer);
            stream.UInt32(ref NumVertices);
            stream.Object(ref IndexBuffer);
            stream.Object(ref WireframeIndexBuffer);
            stream.Object(ref AdjacencyIndexBuffer);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FStaticMeshRenderData) sourceObj;

            RawTriangles = other.RawTriangles;
            Elements = IUnrealSerializable.Clone(other.Elements, sourceArchive, destArchive);
            PositionVertexBuffer = other.PositionVertexBuffer;
            VertexBuffer = other.VertexBuffer;
            ColorVertexBuffer = other.ColorVertexBuffer;
            NumVertices = other.NumVertices;
            IndexBuffer = other.IndexBuffer;
            WireframeIndexBuffer = other.WireframeIndexBuffer;
            AdjacencyIndexBuffer = other.AdjacencyIndexBuffer;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
            foreach (var elem in Elements)
            {
                elem.PopulateDependencies(dependencyIndices);
            }
        }
    }

    public struct FStaticMeshVertexBuffer : IUnrealSerializable
    {
        #region Serialized data

        public uint NumTexCoords;

        public uint Stride;

        public uint NumVertices;

        public bool bUseFullPrecisionUVs; // UBOOL

        public FByteArrayWithSize VertexData;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.UInt32(ref NumTexCoords);
            stream.UInt32(ref Stride);
            stream.UInt32(ref NumVertices);
            stream.BoolAsInt32(ref bUseFullPrecisionUVs);
            stream.Object(ref VertexData);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FStaticMeshVertexBuffer) sourceObj;

            NumTexCoords = other.NumTexCoords;
            Stride = other.Stride;
            NumVertices = other.NumVertices;
            bUseFullPrecisionUVs = other.bUseFullPrecisionUVs;
            VertexData = other.VertexData;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }

    public class UStaticMesh(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public FBoxSphereBounds Bounds;

        [Index(typeof(URB_BodySetup))]
        public int BodySetup;

        public TKDOPTree KDOPTree;

        public int InternalVersion;

        public bool bHaveSourceData; // UBOOL

        public FStaticMeshOptimizationSettings[] OptimizationSettings;

        public bool bHasBeenSimplified; // UBOOL

        public FStaticMeshRenderData[] LODModels;

        public FStaticMeshLODInfo[] LODInfo;

        public FRotator ThumbnailAngle;

        public float ThumbnailDistance;

        public string HighResSourceMeshName;

        public int HighResSourceMeshCRC;

        public Guid LightingGuid;

        public int VertexPositionVersionNumber;

        public float[] CachedStreamingTextureFactors;

        public bool bRemoveDegenerates; // UBOOL

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Object(ref Bounds);
            stream.Int32(ref BodySetup);
            stream.Object(ref KDOPTree);
            stream.Int32(ref InternalVersion);
            stream.BoolAsInt32(ref bHaveSourceData);

#if DEBUG
            if (bHaveSourceData)
            {
                Debugger.Break();
            }
#endif

            stream.Array(ref OptimizationSettings);
            stream.BoolAsInt32(ref bHasBeenSimplified);
            stream.Array(ref LODModels);
            stream.Array(ref LODInfo);
            stream.Object(ref ThumbnailAngle);
            stream.Float32(ref ThumbnailDistance);
            stream.String(ref HighResSourceMeshName);
            stream.Int32(ref HighResSourceMeshCRC);
            stream.Guid(ref LightingGuid);
            stream.Int32(ref VertexPositionVersionNumber);
            stream.Float32Array(ref CachedStreamingTextureFactors);
            stream.BoolAsInt32(ref bRemoveDegenerates);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UStaticMesh) sourceObj;

            Bounds = other.Bounds;
            BodySetup = Archive.MapIndexFromSourceArchive(other.BodySetup, other.Archive);

            KDOPTree = new TKDOPTree();
            KDOPTree.CloneFromOtherArchive(other.KDOPTree, other.Archive, Archive);

            InternalVersion = other.InternalVersion;
            bHaveSourceData = other.bHaveSourceData;
            OptimizationSettings = other.OptimizationSettings;
            bHasBeenSimplified = other.bHasBeenSimplified;
            LODModels = IUnrealSerializable.Clone(other.LODModels, other.Archive, Archive);
            LODInfo = IUnrealSerializable.Clone(other.LODInfo, other.Archive, Archive);
            ThumbnailAngle = other.ThumbnailAngle;
            ThumbnailDistance = other.ThumbnailDistance;
            HighResSourceMeshName = other.HighResSourceMeshName;
            HighResSourceMeshCRC = other.HighResSourceMeshCRC;
            LightingGuid = other.LightingGuid;
            VertexPositionVersionNumber = other.VertexPositionVersionNumber;
            CachedStreamingTextureFactors = other.CachedStreamingTextureFactors;
            bRemoveDegenerates = other.bRemoveDegenerates;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            dependencyIndices.Add(BodySetup);

            foreach (var lodModel in LODModels)
            {
                lodModel.PopulateDependencies(dependencyIndices);
            }
        }
    }
}
