﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Sequences
{
    [FixedSize(24)]
    public struct FSavedTransform : IUnrealSerializable
    {
        #region Serialized data

        public FVector Location;
        public FRotator Rotation;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(out Location);
            stream.Object(out Rotation);
        }
    }

    public class USeqAct_Interp(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        [Index(typeof(UObject))]
        public IDictionary<int, FSavedTransform> SavedActorTransforms;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Map(out SavedActorTransforms);
        }
    }
}
