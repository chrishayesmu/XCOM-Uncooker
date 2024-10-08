﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.Components
{
    public class UDominantDirectionalLightComponent(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public short[] DominantLightShadowMap;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            if (!IsClassDefaultObject())
            {
                stream.Int16Array(ref DominantLightShadowMap);
            }

            base.Serialize(stream);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UDominantDirectionalLightComponent other = (UDominantDirectionalLightComponent)sourceObj;

            DominantLightShadowMap = other.DominantLightShadowMap;
        }
    }
}
