using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Actor;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes
{
    /// <summary>
    /// This is not a true Unreal type; rather, this is a class used by the uncooker to indicate that the object is:
    /// <list type="number">
    ///     <item>A normal <see cref="UObject"/> (i.e. not a class or type definition),</item>
    ///     <item>which appends some data to the end of its object block, and</item>
    ///     <item>the appended data does not need to be modified when copying this object between archives.</item>
    /// </list>
    /// Under these conditions, it is safe to simply copy all binary data from the end of the basic UObject definition,
    /// then output it again during write serialization.
    /// </summary>
    /// <param name="archive"></param>
    /// <param name="tableEntry"></param>
    /// <remarks>
    /// If the third condition is violated (e.g. the binary data actually contains some indices which need to be
    /// remapped), then the resulting export object is going to completely break in the Unreal Editor and/or game.
    /// </remarks>
    public class UAppendedBinaryDataObject(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public byte[] Data;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            int dataSizeBytes = stream.IsWrite ? Data.Length : (ExportTableEntry.SerialOffset + ExportTableEntry.SerialSize) - (int) stream.Position;
            stream.Bytes(ref Data, dataSizeBytes);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UAppendedBinaryDataObject) sourceObj;

            Data = other.Data;
        }
    }
}
