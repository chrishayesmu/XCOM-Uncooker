using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Actor;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.XCom
{
    public struct DebrisMeshInfo : IUnrealSerializable
    {
        #region Serialized data

        public int ColumnIdx;

        [Index(typeof(UObject))]
        public int MeshComponent;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref ColumnIdx); 
            stream.Int32(ref MeshComponent);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (DebrisMeshInfo) sourceObj;

            ColumnIdx = other.ColumnIdx;
            MeshComponent = destArchive.MapIndexFromSourceArchive(other.MeshComponent, sourceArchive);
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
            dependencyIndices.Add(MeshComponent);
        }
    }

    public class XComDestructionInstData(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        // Key is index of a XComDecoFracLevelActor; value is index of a XComFracDecoComponent
        [Index(typeof(UObject))]
        public IDictionary<int, IList<int>> DecoFracToDecoComponents;

        // Key is index of a XComDecoFracLevelActor; value is index of a XComFracDebrisComponent
        [Index(typeof(UObject))]
        public IDictionary<int, IList<int>> DecoFracToDebrisComponents;

        // Key is index of a XComDecoFracLevelActor
        [Index(typeof(UObject))]
        public IDictionary<int, IList<DebrisMeshInfo>> DecoFracToDebrisStaticMeshInfos;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            if (stream.IsRead)
            {
                stream.MultiMap(ref DecoFracToDecoComponents);
                stream.MultiMap(ref DecoFracToDebrisComponents);
                stream.MultiMap(ref DecoFracToDebrisStaticMeshInfos);
            }
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (XComDestructionInstData) sourceObj;

            DecoFracToDecoComponents = new Dictionary<int, IList<int>>();

            foreach (var entry in other.DecoFracToDecoComponents)
            {
                var key = Archive.MapIndexFromSourceArchive(entry.Key, other.Archive);
                var value = Archive.MapIndicesFromSourceArchive(entry.Value, other.Archive);

                DecoFracToDecoComponents[key] = value;
            }

            DecoFracToDebrisComponents = new Dictionary<int, IList<int>>();

            foreach (var entry in other.DecoFracToDebrisComponents)
            {
                var key = Archive.MapIndexFromSourceArchive(entry.Key, other.Archive);
                var value = Archive.MapIndicesFromSourceArchive(entry.Value, other.Archive);

                DecoFracToDebrisComponents[key] = value;
            }

            DecoFracToDebrisStaticMeshInfos = new Dictionary<int, IList<DebrisMeshInfo>>();

            foreach (var entry in other.DecoFracToDebrisStaticMeshInfos)
            {
                var key = Archive.MapIndexFromSourceArchive(entry.Key, other.Archive);
                var value = IUnrealSerializable.Clone(entry.Value, other.Archive, Archive);

                DecoFracToDebrisStaticMeshInfos[key] = value;
            }
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            dependencyIndices.AddRange(DecoFracToDecoComponents.Keys);
            dependencyIndices.AddRange(DecoFracToDecoComponents.Values.SelectMany(x => x));

            dependencyIndices.AddRange(DecoFracToDebrisComponents.Keys);
            dependencyIndices.AddRange(DecoFracToDebrisComponents.Values.SelectMany(x => x));

            dependencyIndices.AddRange(DecoFracToDebrisStaticMeshInfos.Keys);

            foreach (var obj in DecoFracToDebrisStaticMeshInfos.Values.SelectMany(x => x))
            {
                obj.PopulateDependencies(dependencyIndices);
            }
        }
    }
}
