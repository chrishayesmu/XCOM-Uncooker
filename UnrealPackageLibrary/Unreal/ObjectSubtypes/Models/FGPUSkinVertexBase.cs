﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.Models
{
    // Note: it may be that none of these classes are necessary, following the discovery that they're just bulk serialized
    // and can be treated as binary data. All of these could be unused.

    public class FGPUSkinVertexBase
    {
        public int NumTexCoords = 1;

        public FGPUSkinVertexBase() { }

        #region Serialized data

        public FPackedNormal TangentX;

        public FPackedNormal TangentZ;

        public byte[] InfluenceBones = new byte[4]; // fixed size array

        public byte[] InfluenceWeights = new byte[4]; // fixed size array

        #endregion

        public virtual void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref TangentX);
            stream.Object(ref TangentZ);

            stream.Bytes(ref InfluenceBones, 4);
            stream.Bytes(ref InfluenceWeights, 4);
        }
    }

    public class TGPUSkinVertexFloat16Uvs : FGPUSkinVertexBase
    {
        #region Serialized data

        public FVector Position;

        public FVector2DHalf[] UVs;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Object(ref Position);

            for (int i = 0; i < NumTexCoords; i++)
            {
                stream.Object(ref UVs[i]);
            }
        }
    }

    public class TGPUSkinVertexFloat32Uvs : FGPUSkinVertexBase
    {
        #region Serialized data

        public FVector Position;

        public FVector2D[] UVs;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Object(ref Position);

            for (int i = 0; i < NumTexCoords; i++)
            {
                stream.Object(ref UVs[i]);
            }
        }
    }

    public class TGPUSkinVertexFloat16Uvs32Xyz : FGPUSkinVertexBase
    {
        #region Serialized data

        public uint Position; // packed 11:11:10 x:y:z position

        public FVector2DHalf[] UVs;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.UInt32(ref Position);

            for (int i = 0; i < NumTexCoords; i++)
            {
                stream.Object(ref UVs[i]);
            }
        }
    }

    public class TGPUSkinVertexFloat32Uvs32Xyz : FGPUSkinVertexBase
    {
        #region Serialized data

        public uint Position; // packed 11:11:10 x:y:z position

        public FVector2D[] UVs;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.UInt32(ref Position);

            for (int i = 0; i < NumTexCoords; i++)
            {
                stream.Object(ref UVs[i]);
            }
        }
    }
}
