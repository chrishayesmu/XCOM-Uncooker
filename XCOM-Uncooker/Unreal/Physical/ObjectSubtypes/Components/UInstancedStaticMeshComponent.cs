﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Actor;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Components
{
    public class UInstancedStaticMeshComponent(FArchive archive, FObjectTableEntry tableEntry) : UStaticMeshComponent(archive, tableEntry)
    {
        #region Serialized data

        public FByteArrayWithSize PerInstanceSMData;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Object(ref PerInstanceSMData);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UInstancedStaticMeshComponent) sourceObj;

            PerInstanceSMData = other.PerInstanceSMData;
        }
    }
}
