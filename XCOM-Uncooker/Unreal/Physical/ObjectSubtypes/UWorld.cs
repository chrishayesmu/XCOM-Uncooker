using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Components;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes
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
    }
}
