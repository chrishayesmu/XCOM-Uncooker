using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical
{
    public struct FBox : IUnrealSerializable
    {
        #region Serialized data

        public FVector Min;

        public FVector Max;

        public bool IsValid;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref Min);
            stream.Object(ref Max);
            stream.Bool(ref IsValid);
        }
    }

    [FixedSize(4)]
    public struct FColor : IUnrealSerializable
    {
        public byte R, G, B, A;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.UInt8(ref R);
            stream.UInt8(ref G);
            stream.UInt8(ref B);
            stream.UInt8(ref A);
        }
    }

    [FixedSize(12)]
    public struct FGenerationInfo
    {
        public int ExportCount;
        public int NameCount;
        public int NetObjectCount;

        public override string ToString()
        {
            return $"FGenerationInfo=(ExportCount={ExportCount}, NameCount={NameCount}, NetObjectCount={NetObjectCount})";
        }
    }

    /// <summary>
    /// Metadata regarding a thumbnail to be shown in the UE3 Content Browser. Not very relevant for
    /// uncooking, but included here for completeness.
    /// </summary>
    public struct FThumbnailMetadata
    {
        public string ClassName;

        public string ObjectPathWithoutPackageName;

        /// <summary>
        /// Position in the package file where the thumbnail data is stored.
        /// </summary>
        public int FileOffset;
    }

    [FixedSize(4)]
    public struct FPackedNormal : IUnrealSerializable
    {
        public byte X, Y, Z, W;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.UInt8(ref X);
            stream.UInt8(ref Y);
            stream.UInt8(ref Z);
            stream.UInt8(ref W);
        }
    }

    [FixedSize(16)]
    public struct FPlane : IUnrealSerializable
    {
        public float X, Y, Z, W;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Float32(ref X);
            stream.Float32(ref Y);
            stream.Float32(ref Z);
            stream.Float32(ref W);
        }
    }

    [FixedSize(16)]
    public struct FQuat : IUnrealSerializable
    {
        public float X, Y, Z, W;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Float32(ref X);
            stream.Float32(ref Y);
            stream.Float32(ref Z);
            stream.Float32(ref W);
        }
    }

    [FixedSize(12)]
    public struct FRotator : IUnrealSerializable
    {
        public int Pitch, Yaw, Roll;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref Pitch);
            stream.Int32(ref Yaw);
            stream.Int32(ref Roll);
        }
    }

    [FixedSize(16)]
    public struct FSphere : IUnrealSerializable
    {
        public FVector Center;
        public float W;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref Center);
            stream.Float32(ref W);
        }
    }

    [FixedSize(12)]
    public struct FVector : IUnrealSerializable
    {
        public float X, Y, Z;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Float32(ref X);
            stream.Float32(ref Y);
            stream.Float32(ref Z);
        }
    }

    [FixedSize(8)]
    public struct FVector2D : IUnrealSerializable
    {
        public float X, Y;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Float32(ref X);
            stream.Float32(ref Y);
        }
    }

    [FixedSize(4)]
    public struct FVector2DHalf : IUnrealSerializable
    {
        public Half X, Y;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Float16(ref X);
            stream.Float16(ref Y);
        }
    }
}
