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
            stream.Object(out Base);
            stream.Object(out Normal);
            stream.Object(out TextureU);
            stream.Object(out TextureV);
            stream.Array(out Vertices);
            stream.UInt32(out PolyFlags);
            stream.Int32(out Actor);
            stream.Name(out ItemName);
            stream.Int32(out Material);
            stream.Int32(out iLink);
            stream.Int32(out iBrushPoly);
            stream.Float32(out ShadowMapScale);
            stream.UInt32(out LightingChannels);
            stream.Object(out LightmassSettings);
            stream.Name(out RulesetVariation);
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

            stream.Int32(out DbNum);
            stream.Int32(out DbMax);

            // Elements is a transactional array, but for some reason its serialization is handled differently
            // from others, so we have to do some custom logic for this class in particular
            stream.Int32(out Elements.Owner);

            if (stream.IsRead)
            {
                Elements.Data = new FPoly[DbNum];
            }

            for (int i = 0; i < DbNum; i++)
            {
                stream.Object(out Elements.Data[i]);
            }
        }
    }
}
