﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.Intrinsic.Core.Properties;
using UnrealArchiveLibrary.Unreal.SerializedProperties.ImmutableWhenCooked;

namespace UnrealArchiveLibrary.Unreal.SerializedProperties
{
    public class USerializedArrayProperty(FArchive archive, UProperty prop, FPropertyTag? tag) : USerializedProperty(archive, prop, tag)
    {
        public override string TagType => "ArrayProperty";

        public override bool HasDefaultValueForType => NumElements == 0;

        #region Serialized data

        public int NumElements;

        public USerializedProperty[] Data;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref NumElements);

            if (stream.IsRead)
            {
                Data = new USerializedProperty[NumElements];
            }

            // TODO this is horribly inefficient for large byte arrays
            for (int i = 0; i < NumElements; i++)
            {
                if (stream.IsRead)
                {
                    UProperty innerProp = (BackingProperty as UArrayProperty).Inner;
                    Data[i] = innerProp.CreateSerializedProperty(Archive, null);
                }

                Data[i].Serialize(stream);
            }
        }

        public override USerializedProperty CloneToOtherArchive(FArchive destArchive)
        {
            var tag = ClonePropertyTag(destArchive);
            var other = new USerializedArrayProperty(destArchive, BackingProperty, tag);

            other.NumElements = NumElements;

            other.Data = new USerializedProperty[Data.Length];

            for (int i = 0; i < Data.Length; i++)
            {
                other.Data[i] = Data[i].CloneToOtherArchive(destArchive);
            }

            return other;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            foreach (var prop in Data)
            {
                prop.PopulateDependencies(dependencyIndices);
            }
        }
    }
}
