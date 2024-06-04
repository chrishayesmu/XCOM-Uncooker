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
    public class XComWorldData(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        // Index of an XComWorldDataContainer
        [Index(typeof(UObject))]
        public int WorldDataPtr;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            if (stream.IsRead)
            {
                // This is a native prop in XCOM, so we can only read it but not write it
                stream.Int32(ref WorldDataPtr);
            }
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            // Don't try to map the WorldDataPtr; it can't exist in an uncooked archive
        }
    }
}
