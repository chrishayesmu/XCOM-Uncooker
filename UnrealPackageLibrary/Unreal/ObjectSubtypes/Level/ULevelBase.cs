using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Actor;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.Level
{
    /// <summary>
    /// This struct just exists to be used as a generic type for TTransactionalArray.
    /// </summary>
    public struct ActorPointer : IUnrealSerializable
    {
        #region Serialized data
        
        [Index(typeof(UObject))]
        public int Index;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref Index);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (ActorPointer) sourceObj;

            Index = destArchive.MapIndexFromSourceArchive(other.Index, sourceArchive);
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
            dependencyIndices.Add(Index);
        }
    }

    public class ULevelBase(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        [Index(typeof(UObject))]
        public TTransactionalArray<ActorPointer> Actors;

        public FURL Url;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Object(ref Actors);
            stream.Object(ref Url);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (ULevelBase) sourceObj;

            Actors = new TTransactionalArray<ActorPointer>();
            Actors.CloneFromOtherArchive(other.Actors, other.Archive, Archive);

            Url = other.Url;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            Actors.PopulateDependencies(dependencyIndices);

            foreach (var actor in Actors.Data)
            {
                actor.PopulateDependencies(dependencyIndices);
            }
        }
    }
}
