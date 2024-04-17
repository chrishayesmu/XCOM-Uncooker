﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties
{
    /// <summary>
    /// Represents a UE3 dynamic string. The property has no additional metadata, since
    /// anything there is to know abref it will be determined at runtime.
    /// </summary>
    public class UStrProperty(FArchive archive, FObjectTableEntry tableEntry) : UProperty(archive, tableEntry)
    {
        public override USerializedProperty CreateSerializedProperty(FArchive archive, FPropertyTag? tag)
        {
            return new USerializedStringProperty(archive, this, tag);
        }
    }
}
