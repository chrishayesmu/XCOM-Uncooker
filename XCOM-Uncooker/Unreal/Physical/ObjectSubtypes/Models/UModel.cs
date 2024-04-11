using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Physics;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Models
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
            stream.Int32(out ZoneActor);
            stream.UInt64(out Connectivity);
            stream.UInt64(out Visibility);
            stream.Float32(out LastRenderTime);
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

            stream.Object(out Bounds);
            stream.BulkArray(out Vectors, 12);
            stream.BulkArray(out Points, 12);
            stream.BulkArray(out Nodes, 64);
            stream.Object(out Surfs);
            stream.BulkArray(out Verts, 24);
            stream.Int32(out NumSharedSides);
            stream.Array(out Zones);
            stream.Int32(out Polys);
            stream.BulkArray(out LeafHulls);
            stream.BulkArray(out Leaves);
            stream.BoolAsInt32(out RootOutside);
            stream.BoolAsInt32(out Linked);
            stream.BulkArray(out PortalNodes);
            stream.UInt32(out NumUniqueVertices);
            stream.Object(out VertexBuffer);
            stream.Guid(out LightingGuid);
            stream.Array(out LightmassSettings);
        }
    }
}
