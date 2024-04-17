using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Actor;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Sequences
{
    [FixedSize(24)]
    public struct FSavedTransform : IUnrealSerializable
    {
        #region Serialized data

        public FVector Location;
        public FRotator Rotation;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Object(ref Location);
            stream.Object(ref Rotation);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FSavedTransform) sourceObj;

            Location = other.Location;
            Rotation = other.Rotation;
        }
    }

    public class USeqAct_Interp(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        [Index(typeof(UObject))]
        public IDictionary<int, FSavedTransform> SavedActorTransforms;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Map(ref SavedActorTransforms);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (USeqAct_Interp) sourceObj;

            SavedActorTransforms = new Dictionary<int, FSavedTransform>();

            foreach (var entry in other.SavedActorTransforms)
            {
                var key = Archive.MapIndexFromSourceArchive(entry.Key, other.Archive);
                var value = entry.Value;

                SavedActorTransforms[key] = value;
            }
        }
    }
}
