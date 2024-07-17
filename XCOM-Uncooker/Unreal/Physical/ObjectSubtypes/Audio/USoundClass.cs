using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Actor;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Audio
{
    public struct FSoundClassEditorData : IUnrealSerializable
    {
        #region Serialized data

        public int NodePosX;

        public int NodePosY;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref NodePosX);
            stream.Int32(ref NodePosY);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FSoundClassEditorData) sourceObj;

            NodePosX = other.NodePosX;
            NodePosY = other.NodePosY;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }

    public class USoundClass(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        [Index(typeof(USoundClass))]
        public IDictionary<int, FSoundClassEditorData> EditorData;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Map(ref EditorData);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (USoundClass) sourceObj;

            EditorData = new Dictionary<int, FSoundClassEditorData>();

            foreach (var entry in other.EditorData)
            {
                int key = Archive.MapIndexFromSourceArchive(entry.Key, other.Archive);
                var value = entry.Value;

                EditorData.Add(key, value);
            }
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            dependencyIndices.AddRange(EditorData.Keys);
        }
    }
}
