﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Materials
{
    public class FNormalParameter : IUnrealSerializable
    {
        #region Serialized data

        public FName ParameterName;

        public byte CompressionSettings;

        public bool Override;

        public Guid ExpressionGuid;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Name(ref ParameterName);
            stream.UInt8(ref CompressionSettings);
            stream.BoolAsInt32(ref Override);
            stream.Guid(ref ExpressionGuid);
        }
    }
}
