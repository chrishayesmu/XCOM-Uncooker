using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

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
            stream.Int32Array(out Actors);
            stream.Int32Array(out ChildGroups);
            stream.Int32(out PackedBools);
        }
    }

    public class XComVisGroupData(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        // public MultiMap_Mirror ActorToActorGroups;

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

            stream.Map(out ActorToActorGroups);
            stream.Array(out VisGroupEntries);
        }
    }
}
// Act1_IntroLevel.TheWorld.PersistentLevel.WorldInfo_3.XComVisGroupData_0