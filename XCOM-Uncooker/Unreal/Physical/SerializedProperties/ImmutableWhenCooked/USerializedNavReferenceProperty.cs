﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;

namespace XCOM_Uncooker.Unreal.Physical.SerializedProperties.ImmutableWhenCooked
{
    public class USerializedNavReferenceProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "StructProperty";

        #region Serialized data

        [Index(typeof(UObject))]
        public int Nav;

        public Guid Guid;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref Nav);
            stream.Guid(ref Guid);
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedNavReferenceProperty(destArchive, null, tag);

            other.Nav = destArchive.MapIndexFromSourceArchive(Nav, Archive);
            other.Guid = Guid;

            return other;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            dependencyIndices.Add(Nav);
        }
    }
}
