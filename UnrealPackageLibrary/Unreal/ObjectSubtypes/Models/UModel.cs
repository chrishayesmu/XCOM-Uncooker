﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Actor;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Physics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.Models
{
    public struct FZoneProperties : IUnrealSerializable
    {
        #region Serialized data

        [Index(typeof(UObject))]
        public int ZoneActor;

        public ulong Connectivity;

        public ulong Visibility;

        public float LastRenderTime;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref ZoneActor);
            stream.UInt64(ref Connectivity);
            stream.UInt64(ref Visibility);
            stream.Float32(ref LastRenderTime);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FZoneProperties) sourceObj;

            ZoneActor = destArchive.MapIndexFromSourceArchive(other.ZoneActor, sourceArchive);
            Connectivity = other.Connectivity;
            Visibility = other.Visibility;
            LastRenderTime = other.LastRenderTime;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
            dependencyIndices.Add(ZoneActor);
        }
    }

    public class UModel(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public FBoxSphereBounds Bounds;

        public FVector[] Vectors;

        public FVector[] Points;

        public FBspNode[] Nodes;

        public TTransactionalArray<FBspSurf> Surfs;

        public FVert[] Verts;

        public int NumSharedSides;

        public FZoneProperties[] Zones;

        [Index(typeof(UObject))]
        public int Polys;

        public int[] LeafHulls;

        public int[] Leaves;

        public bool RootOutside; // UBOOL

        public bool Linked; // UBOOL

        public int[] PortalNodes;

        public uint NumUniqueVertices;

        public FModelVertexBuffer VertexBuffer;

        public Guid LightingGuid;

        public FLightmassPrimitiveSettings[] LightmassSettings;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Object(ref Bounds);
            stream.BulkArray(ref Vectors, 12);
            stream.BulkArray(ref Points, 12);
            stream.BulkArray(ref Nodes, 64);
            stream.Object(ref Surfs);
            stream.BulkArray(ref Verts, 24);
            stream.Int32(ref NumSharedSides);
            stream.Array(ref Zones);
            stream.Int32(ref Polys);
            stream.BulkArray(ref LeafHulls);
            stream.BulkArray(ref Leaves);
            stream.BoolAsInt32(ref RootOutside);
            stream.BoolAsInt32(ref Linked);
            stream.BulkArray(ref PortalNodes);
            stream.UInt32(ref NumUniqueVertices);
            stream.Object(ref VertexBuffer);
            stream.Guid(ref LightingGuid);
            stream.Array(ref LightmassSettings);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UModel) sourceObj;

            Bounds = other.Bounds;
            Vectors = other.Vectors;
            Points = other.Points;
            Nodes = other.Nodes;

            Surfs = new TTransactionalArray<FBspSurf>();
            Surfs.CloneFromOtherArchive(other.Surfs, other.Archive, Archive);

            Verts = other.Verts;
            NumSharedSides = other.NumSharedSides;
            Zones = IUnrealSerializable.Clone(other.Zones, other.Archive, Archive);
            Polys = Archive.MapIndexFromSourceArchive(other.Polys, other.Archive);
            LeafHulls = other.LeafHulls;
            Leaves = other.Leaves;
            RootOutside = other.RootOutside;
            Linked = other.Linked;
            PortalNodes = other.PortalNodes;
            NumUniqueVertices = other.NumUniqueVertices;
            VertexBuffer = other.VertexBuffer;
            LightingGuid = other.LightingGuid;
            LightmassSettings = other.LightmassSettings;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            Surfs.PopulateDependencies(dependencyIndices);
            
            foreach (var zone in Zones)
            {
                zone.PopulateDependencies(dependencyIndices);
            }
            
            dependencyIndices.Add(Polys);
        }
    }
}
