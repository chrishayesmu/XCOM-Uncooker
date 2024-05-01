using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical
{
    [FixedSize(25)]
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FBox) sourceObj;

            Min = other.Min;
            Max = other.Max;
            IsValid = other.IsValid;
        }
    }

    public struct FCompressedChunkInfo : IUnrealSerializable
    {
        #region Serialized data

        public int CompressedSize;

        public int UncompressedSize;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref CompressedSize);
            stream.Int32(ref UncompressedSize);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FCompressedChunkInfo) sourceObj;

            CompressedSize = other.CompressedSize;
            UncompressedSize = other.UncompressedSize;
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FColor) sourceObj;

            R = other.R;
            G = other.G;
            B = other.B;
            A = other.A;
        }
    }

    [FixedSize(12)]
    public struct FGenerationInfo : IUnrealSerializable
    {
        public int ExportCount;
        public int NameCount;
        public int NetObjectCount;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref ExportCount);
            stream.Int32(ref NameCount);
            stream.Int32(ref NetObjectCount);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FGenerationInfo) sourceObj;

            ExportCount = other.ExportCount;
            NameCount = other.NameCount;
            NetObjectCount = other.NetObjectCount;
        }

        public override string ToString()
        {
            return $"FGenerationInfo=(ExportCount={ExportCount}, NameCount={NameCount}, NetObjectCount={NetObjectCount})";
        }
    }

    /// <summary>
    /// Metadata regarding a thumbnail to be shown in the UE3 Content Browser. Not very relevant for
    /// uncooking, but included here for completeness.
    /// </summary>
    public struct FThumbnailMetadata : IUnrealSerializable
    {
        public string ClassName;

        public string ObjectPathWithoutPackageName;

        /// <summary>
        /// Position in the package file where the thumbnail data is stored.
        /// </summary>
        public int FileOffset;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.String(ref ClassName);
            stream.String(ref ObjectPathWithoutPackageName);
            stream.Int32(ref FileOffset);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FThumbnailMetadata) sourceObj;

            ClassName = other.ClassName;
            ClassName = other.ObjectPathWithoutPackageName;
            FileOffset = other.FileOffset;
        }
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FPackedNormal) sourceObj;

            X = other.X;
            Y = other.Y;
            Z = other.Z;
            W = other.W;
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FPlane) sourceObj;

            X = other.X;
            Y = other.Y;
            Z = other.Z;
            W = other.W;
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FQuat) sourceObj;

            X = other.X;
            Y = other.Y;
            Z = other.Z;
            W = other.W;
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FRotator) sourceObj;

            Pitch = other.Pitch;
            Yaw = other.Yaw;
            Roll = other.Roll;
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FSphere) sourceObj;

            Center = other.Center;
            W = other.W;
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FVector) sourceObj;

            X = other.X;
            Y = other.Y;
            Z = other.Z;
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FVector2D) sourceObj;

            X = other.X;
            Y = other.Y;
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

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FVector2DHalf) sourceObj;

            X = other.X;
            Y = other.Y;
        }
    }
}
