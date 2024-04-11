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
            stream.Object(out Min);
            stream.Object(out Max);
            stream.Bool(out IsValid);
        }
    }

    [FixedSize(4)]
    public struct FColor : IUnrealSerializable
    {
        public byte R, G, B, A;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.UInt8(out R);
            stream.UInt8(out G);
            stream.UInt8(out B);
            stream.UInt8(out A);
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
            stream.UInt8(out X);
            stream.UInt8(out Y);
            stream.UInt8(out Z);
            stream.UInt8(out W);
        }
    }

    [FixedSize(16)]
    public struct FPlane : IUnrealSerializable
    {
        public float X, Y, Z, W;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Float32(out X);
            stream.Float32(out Y);
            stream.Float32(out Z);
            stream.Float32(out W);
        }
    }

    [FixedSize(16)]
    public struct FQuat : IUnrealSerializable
    {
        public float X, Y, Z, W;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Float32(out X);
            stream.Float32(out Y);
            stream.Float32(out Z);
            stream.Float32(out W);
        }
    }

    [FixedSize(12)]
    public struct FRotator : IUnrealSerializable
    {
        public int Pitch, Yaw, Roll;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(out Pitch);
            stream.Int32(out Yaw);
            stream.Int32(out Roll);
        }
    }

    [FixedSize(16)]
    public struct FSphere : IUnrealSerializable
    {
        public FVector Center;
        public float W;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(out Center);
            stream.Float32(out W);
        }
    }

    [FixedSize(12)]
    public struct FVector : IUnrealSerializable
    {
        public float X, Y, Z;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Float32(out X);
            stream.Float32(out Y);
            stream.Float32(out Z);
        }
    }

    [FixedSize(8)]
    public struct FVector2D : IUnrealSerializable
    {
        public float X, Y;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Float32(out X);
            stream.Float32(out Y);
        }
    }

    [FixedSize(4)]
    public struct FVector2DHalf : IUnrealSerializable
    {
        public Half X, Y;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Float16(out X);
            stream.Float16(out Y);
        }
    }
}
