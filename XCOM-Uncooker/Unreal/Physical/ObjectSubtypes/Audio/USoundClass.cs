﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Audio
{
    public struct FSoundClassEditorData : IUnrealSerializable
    {
        #region Serialized data

        public int NodePosX;

        public int NodePosY;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(out NodePosX);
            stream.Int32(out NodePosY);
        }
    }

    public class USoundClass(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public IDictionary<int, FSoundClassEditorData> EditorData;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Map(out EditorData);
        }
    }
}
