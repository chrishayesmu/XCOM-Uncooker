﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.Intrinsic.Core.Properties;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Actor;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.XCom
{
    public class XComCharacterVoiceBank(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        // Maps from XComGame.XGGameData.ECharacterSpeech to the corresponding UProperty within the
        // XComCharacterVoiceBank class. Why this exists is uncertain, since custom voice packs work fine
        // without this data; probably it's a simple optimization.
        [Index(typeof(UProperty))]
        public IDictionary<byte, int> EventToPropertyMap;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            // When deserializing, we can't write this data or it'll crash the UDK due to unread data
            if (stream.IsRead)
            {
                stream.Map(ref EventToPropertyMap);
            }
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (XComCharacterVoiceBank) sourceObj;

            EventToPropertyMap = new Dictionary<byte, int>();

            foreach (var entry in other.EventToPropertyMap)
            {
                var key = entry.Key;
                var value = Archive.MapIndexFromSourceArchive(entry.Value, other.Archive);

                EventToPropertyMap[key] = value;
            }
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            dependencyIndices.AddRange(EventToPropertyMap.Values);
        }
    }
}
