using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Audio
{
    public struct FSoundNodeEditorData
    {
        public int NodePosX;
        public int NodePosY;
    }

    /// <summary>
    /// Representation of the SoundCue class. When deserializing archives, this is basically just a <see cref="UObject"/> with
    /// 4 extra bytes at the end, because the only non-property data is editor-only and gets stripped out as part of the cooking
    /// process. We represent it as its own class so that we can potentially try to insert some editor-only data of our own later on.
    /// </summary>
    /// <param name="archive"></param>
    /// <param name="tableEntry"></param>
    public class USoundCue(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized Data

        // Key here is the index of a USoundNode
        public IDictionary<int, FSoundNodeEditorData> EditorData = new Dictionary<int, FSoundNodeEditorData>();

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            // CDO doesn't serialize any of this stuff
            if (IsClassDefaultObject())
            {
                return;
            }

            int numEditorDataEntries;

            if (stream.IsWrite)
            {
                numEditorDataEntries = EditorData.Count;
            }

            stream.Int32(out numEditorDataEntries);

            // For reads: just pull N entries
            if (stream.IsRead)
            {
                for (int i = 0; i < numEditorDataEntries; i++)
                {
                    FSoundNodeEditorData data = new FSoundNodeEditorData();

                    stream.Int32(out int key);
                    stream.Int32(out data.NodePosX);
                    stream.Int32(out data.NodePosY);

                    EditorData.Add(key, data);
                }
            }
            else
            {
                // For writes, iterate the map
                foreach (var entry in EditorData)
                {
                    int key = entry.Key;
                    int nodeX = entry.Value.NodePosX;
                    int nodeY = entry.Value.NodePosY;

                    stream.Int32(out key);
                    stream.Int32(out nodeX);
                    stream.Int32(out nodeY);
                }
            }
        }
    }
}
