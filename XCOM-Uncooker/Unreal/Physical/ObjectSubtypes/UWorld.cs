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
            stream.Object(out CamPosition);
            stream.Object(out CamRotation);
            stream.Float32(out CamOrthoZoom);
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

            stream.Int32(out PersistentLevel);
            stream.Int32(out PersistentFaceFXAnimSet);

            stream.Object(out EditorViews[0]);
            stream.Object(out EditorViews[1]);
            stream.Object(out EditorViews[2]);
            stream.Object(out EditorViews[3]);

            stream.Int32(out SaveGameSummary);
            stream.Int32Array(out ExtraReferencedObjects);
        }
    }
}
