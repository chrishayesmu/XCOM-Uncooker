﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Lighting;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Components
{
    public struct FColorVertexBuffer : IUnrealSerializable
    {
        #region Serialized data

        public uint Stride;

        public uint NumVertices;

        public byte[] VertexData;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.UInt32(ref Stride);
            stream.UInt32(ref NumVertices);

            if (NumVertices > 0)
            {
                stream.BulkArray(ref VertexData);
            }
        }
    }

    public struct FStaticMeshComponentLODInfo : IUnrealSerializable
    {
        public FStaticMeshComponentLODInfo() {}

        #region Serialized data

        [Index(typeof(UObject))]
        public int[] ShadowMaps;

        [Index(typeof(UObject))]
        public int[] ShadowVertexBuffers;

        public FLightMap LightMap = new FLightMap();

        public bool bLoadVertexColorData;

        public FColorVertexBuffer OverrideVertexColors;

        public FVector[] VertexColorPositions;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32Array(ref ShadowMaps);
            stream.Int32Array(ref ShadowVertexBuffers);
            stream.Object(ref LightMap);
            stream.Bool(ref bLoadVertexColorData);

            if (bLoadVertexColorData)
            {
                stream.Object(ref OverrideVertexColors);
            }

            stream.Array(ref VertexColorPositions);
        }
    }

    public class UStaticMeshComponent(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public FStaticMeshComponentLODInfo[] LODData;

        public FStaticMeshComponentLODInfo[] SwapMeshData = new FStaticMeshComponentLODInfo[2]; // XCOM addition; fixed size array

        [Index(typeof(UObject))]
        public int[] SwapStaticMeshes = new int[2]; // XCOM addition; fixed size array

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Array(ref LODData);
            stream.Object(ref SwapMeshData[0]);
            stream.Object(ref SwapMeshData[1]);
            stream.Int32(ref SwapStaticMeshes[0]);
            stream.Int32(ref SwapStaticMeshes[1]);
        }
    }
}
