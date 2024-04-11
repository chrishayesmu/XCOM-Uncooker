using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Actor
{
    public class APrefabInstance(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        // Both key and values are indices
        [Index(typeof(UObject))]
        public IDictionary<int, int> ArchetypeToInstanceMap;

        // Only the key is an index
        [Index(typeof(UObject))]
        public IDictionary<int, int> PI_ObjectMap;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Map(ref ArchetypeToInstanceMap);
            stream.Map(ref PI_ObjectMap);
        }
    }
}
