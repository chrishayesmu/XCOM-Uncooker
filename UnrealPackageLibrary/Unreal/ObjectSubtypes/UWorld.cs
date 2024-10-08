﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Actor;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Components;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes
{
    public struct FLevelViewportInfo : IUnrealSerializable
    {
        #region Serialized data

        public FVector CamPosition;
        public FRotator CamRotation;
        public float CamOrthoZoom;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref CamPosition);
            stream.Object(ref CamRotation);
            stream.Float32(ref CamOrthoZoom);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FLevelViewportInfo) sourceObj;

            CamPosition = other.CamPosition;
            CamRotation = other.CamRotation;
            CamOrthoZoom = other.CamOrthoZoom;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }

    public class UWorld(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        [Index(typeof(UObject))]
        public int PersistentLevel;

        [Index(typeof(UObject))]
        public int PersistentFaceFXAnimSet;

        public FLevelViewportInfo[] EditorViews = new FLevelViewportInfo[4];

        [Index(typeof(UObject))]
        public int SaveGameSummary;

        [Index(typeof(UObject))]
        public int[] ExtraReferencedObjects;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(ref PersistentLevel);
            stream.Int32(ref PersistentFaceFXAnimSet);

            stream.Object(ref EditorViews[0]);
            stream.Object(ref EditorViews[1]);
            stream.Object(ref EditorViews[2]);
            stream.Object(ref EditorViews[3]);

            stream.Int32(ref SaveGameSummary);
            stream.Int32Array(ref ExtraReferencedObjects);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UWorld) sourceObj;

            PersistentLevel = Archive.MapIndexFromSourceArchive(other.PersistentLevel, other.Archive);
            PersistentFaceFXAnimSet = Archive.MapIndexFromSourceArchive(other.PersistentFaceFXAnimSet, other.Archive);
            EditorViews = other.EditorViews;
            SaveGameSummary = Archive.MapIndexFromSourceArchive(other.SaveGameSummary, other.Archive);
            ExtraReferencedObjects = Archive.MapIndicesFromSourceArchive(other.ExtraReferencedObjects, other.Archive);
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            dependencyIndices.Add(PersistentLevel);
            dependencyIndices.Add(PersistentFaceFXAnimSet);
            dependencyIndices.Add(SaveGameSummary);
            dependencyIndices.AddRange(ExtraReferencedObjects);
        }
    }
}
