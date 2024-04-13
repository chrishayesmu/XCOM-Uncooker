using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Physics;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Models
{
    public struct FPoly : IUnrealSerializable
    {
        #region Serialized data

        public FVector Base;
        public FVector Normal;
        public FVector TextureU;
        public FVector TextureV;
        public FVector[] Vertices;
        public uint PolyFlags;

        [Index(typeof(UObject))]
        public int Actor;

        public FName ItemName;

        [Index(typeof(UObject))]
        public int Material;

        public int iLink;
        public int iBrushPoly;
        public float ShadowMapScale;
        public uint LightingChannels;
        public FLightmassPrimitiveSettings LightmassSettings;
        public FName RulesetVariation;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref Base);
            stream.Object(ref Normal);
            stream.Object(ref TextureU);
            stream.Object(ref TextureV);
            stream.Array(ref Vertices);
            stream.UInt32(ref PolyFlags);
            stream.Int32(ref Actor);
            stream.Name(ref ItemName);
            stream.Int32(ref Material);
            stream.Int32(ref iLink);
            stream.Int32(ref iBrushPoly);
            stream.Float32(ref ShadowMapScale);
            stream.UInt32(ref LightingChannels);
            stream.Object(ref LightmassSettings);
            stream.Name(ref RulesetVariation);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FPoly) sourceObj;

            Base = other.Base;
            Normal = other.Normal;
            TextureU = other.TextureU;
            TextureV = other.TextureV;
            Vertices = other.Vertices;
            PolyFlags = other.PolyFlags;
            Actor = destArchive.MapIndexFromSourceArchive(other.Actor, sourceArchive);
            ItemName = destArchive.MapNameFromSourceArchive(other.ItemName);
            Material = destArchive.MapIndexFromSourceArchive(other.Material, sourceArchive);
            iLink = other.iLink;
            iBrushPoly = other.iBrushPoly;
            ShadowMapScale = other.ShadowMapScale;
            LightingChannels = other.LightingChannels;
            LightmassSettings = other.LightmassSettings;
            RulesetVariation = destArchive.MapNameFromSourceArchive(other.RulesetVariation);
        }
    }

    public class UPolys(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public int DbNum;

        public int DbMax;

        public TTransactionalArray<FPoly> Elements = new();

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(ref DbNum);
            stream.Int32(ref DbMax);

            // Elements is a transactional array, but for some reason its serialization is handled differently
            // from others, so we have to do some custom logic for this class in particular
            stream.Int32(ref Elements.Owner);

            if (stream.IsRead)
            {
                Elements.Data = new FPoly[DbNum];
            }

            for (int i = 0; i < DbNum; i++)
            {
                stream.Object(ref Elements.Data[i]);
            }
        }
    }
}
