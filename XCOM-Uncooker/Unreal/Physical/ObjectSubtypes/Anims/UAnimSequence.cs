using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Anims
{
    public struct FRawAnimSequenceTrack : IUnrealSerializable
    {
        #region Serialized data

        public FVector[] PosKeys;

        public FQuat[] RotKeys;

        #endregion

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FRawAnimSequenceTrack) sourceObj;

            PosKeys = other.PosKeys;
            RotKeys = other.RotKeys;
        }

        public void Serialize(IUnrealDataStream stream)
        {
            stream.BulkArray(ref PosKeys, 12);
            stream.BulkArray(ref RotKeys, 16);
        }
    }

    public class UAnimSequence(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public FRawAnimSequenceTrack[] RawAnimationData;

        public byte[] SerializedData;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Array(ref RawAnimationData);

            // XCOM adds an int property called CompressedStreamRefIndex. If CompressedStreamRefIndex != -1, then
            // the animation data is exactly the same as another AnimSequence in the same AnimSet, and the index can
            // be used to retrieve it. Our UDK doesn't understand this, so we have to undo it.
            if (stream.IsRead)
            {
                var indexProp = GetSerializedProperty("CompressedStreamRefIndex") as USerializedIntProperty;

                if (indexProp == null || indexProp.Value < 0)
                {
                    stream.ByteArray(ref SerializedData);
                }
            }
            else
            {
                stream.ByteArray(ref SerializedData);
            }
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UAnimSequence) sourceObj;

            RawAnimationData = other.RawAnimationData;

            var indexProp = GetSerializedProperty("CompressedStreamRefIndex") as USerializedIntProperty;

            if (indexProp != null && indexProp.Value != -1)
            {
                // Go find the data in the AnimSet and copy it into this object. Use the source object's Outer,
                // because the cloned-into object may not have its Outer set up yet.
                var animSet = sourceObj.Outer;

#if DEBUG
                if (animSet.TableEntry.ClassName != "AnimSet")
                {
                    Debugger.Break();
                }
#endif

                var sequencesProp = animSet.GetSerializedProperty("Sequences") as USerializedArrayProperty;

#if DEBUG
                if (sequencesProp == null || sequencesProp.NumElements < indexProp.Value)
                {
                    Debugger.Break();
                }
#endif

                var sourceAnimSequenceProp = sequencesProp.Data[indexProp.Value] as USerializedObjectProperty;
                var sourceAnimSequence = sourceAnimSequenceProp.Archive.GetObjectByIndex(sourceAnimSequenceProp.ObjectIndex) as UAnimSequence;

#if DEBUG
                if (sourceAnimSequence == null)
                {
                    Debugger.Break();
                }
#endif

                SerializedData = sourceAnimSequence.SerializedData;
            }
            else
            {
                SerializedData = other.SerializedData;
            }
        }
    }
}
