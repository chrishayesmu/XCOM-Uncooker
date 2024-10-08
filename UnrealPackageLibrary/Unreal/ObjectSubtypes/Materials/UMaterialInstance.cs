﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Actor;
using UnrealArchiveLibrary.Unreal.SerializedProperties;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.Materials
{
    /// <summary>
    /// Some form of custom data inside MaterialInstance, which appears to be unique to XCOM. Exactly what it is
    /// remains unclear, but enough is known to be able to deserialize it.
    /// </summary>
    public struct FXComMaterialData : IUnrealSerializable
    {
        #region Serialized data

        public byte[] UnknownHeader;

        public int NumBodySections;

        public byte[] UnknownBody;

        public int UnknownSuffix;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            if (stream.IsRead)
            {
                stream.Bytes(ref UnknownHeader, 16);

#if DEBUG
                for (int i = 0; i < UnknownHeader.Length; i++)
                {
                    if (UnknownHeader[i] != 0)
                    {
                        var reader = stream as UnrealDataReader;
                        reader?.Archive?.Log.LogDebug("Found non-zero header section in archive {FileName}", reader.Archive.FileName);
                        break;
                    }
                }
#endif

                stream.Int32(ref NumBodySections);
                int bodySize = 16 * NumBodySections;
                stream.Bytes(ref UnknownBody, bodySize);

                stream.Int32(ref UnknownSuffix);
            }
            else
            {
                // Don't write anything when serializing out, because the UDK won't be able to handle it
            }
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FXComMaterialData) sourceObj;

            other.UnknownHeader = UnknownHeader;
            other.NumBodySections = NumBodySections;
            other.UnknownBody = UnknownBody;
            other.UnknownSuffix = UnknownSuffix;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }

    public class UMaterialInstance(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public FMaterial MaterialResource_MSP_SM3 = new FMaterial();

        public FXComMaterialData UnknownData = new FXComMaterialData();

        public FStaticParameterSet StaticParameters_MSP_SM3 = new FStaticParameterSet();

        #endregion

        public bool bHasStaticPermutationResource;

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            var prop = (USerializedBoolProperty) GetSerializedProperty("bHasStaticPermutationResource");
            bHasStaticPermutationResource = prop?.BoolValue ?? false;

            if (bHasStaticPermutationResource)
            {
                MaterialResource_MSP_SM3.Serialize(stream);
                UnknownData.Serialize(stream);
                StaticParameters_MSP_SM3.Serialize(stream);
            }
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UMaterialInstance) sourceObj;

            bHasStaticPermutationResource = other.bHasStaticPermutationResource;

            if (bHasStaticPermutationResource)
            {
                MaterialResource_MSP_SM3.CloneFromOtherArchive(other.MaterialResource_MSP_SM3, other.Archive, Archive);
                UnknownData.CloneFromOtherArchive(other.UnknownData, other.Archive, Archive);
                StaticParameters_MSP_SM3.CloneFromOtherArchive(other.StaticParameters_MSP_SM3, other.Archive, Archive);
            }
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            MaterialResource_MSP_SM3.PopulateDependencies(dependencyIndices);
        }
    }
}
