﻿using System;
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

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (APrefabInstance) sourceObj;

            ArchetypeToInstanceMap = new Dictionary<int, int>();
            PI_ObjectMap = new Dictionary<int, int>();

            foreach (var entry in other.ArchetypeToInstanceMap)
            {
                int key = Archive.MapIndexFromSourceArchive(entry.Key, other.Archive);
                int value = Archive.MapIndexFromSourceArchive(entry.Value, other.Archive);

                ArchetypeToInstanceMap.Add(key, value);
            }

            foreach (var entry in other.PI_ObjectMap)
            {
                int key = Archive.MapIndexFromSourceArchive(entry.Key, other.Archive);
                int value = entry.Value;

                PI_ObjectMap.Add(key, value);
            }
        }
    }
}
