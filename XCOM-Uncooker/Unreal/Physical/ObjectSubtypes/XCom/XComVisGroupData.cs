using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Actor;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.XCom
{
    public struct VisGroupEntry : IUnrealSerializable
    {
        #region Serialized data

        [Index(typeof(UObject))]
        public int[] Actors;

        public int[] ChildGroups;

        public int PackedBools;

        #endregion
     
        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32Array(ref Actors);
            stream.Int32Array(ref ChildGroups);
            stream.Int32(ref PackedBools);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (VisGroupEntry) sourceObj;

            Actors = destArchive.MapIndicesFromSourceArchive(other.Actors, sourceArchive);
            ChildGroups = other.ChildGroups;
            PackedBools = other.PackedBools;
        }
    }

    public class XComVisGroupData(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        // Key is index of an Actor; values are array indices within the VisActorGroups script property
        [Index(typeof(UObject))]
        public IDictionary<int, int[]> ActorToActorGroups;

        // Despite being a script property and populated that way, the VisGroupEntry data appears to also
        // be serialized natively for some reason
        public VisGroupEntry[] VisGroupEntries;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            if (stream.IsRead)
            {
                stream.Map(ref ActorToActorGroups);
                stream.Array(ref VisGroupEntries);
            }
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (XComVisGroupData) sourceObj;

            ActorToActorGroups = new Dictionary<int, int[]>();

            foreach (var entry in other.ActorToActorGroups)
            {
                var key = Archive.MapIndexFromSourceArchive(entry.Key, other.Archive);
                var value = Archive.MapIndicesFromSourceArchive(entry.Value, other.Archive);

                ActorToActorGroups[key] = value;
            }

            VisGroupEntries = IUnrealSerializable.Clone(other.VisGroupEntries, other.Archive, Archive);
        }
    }
}