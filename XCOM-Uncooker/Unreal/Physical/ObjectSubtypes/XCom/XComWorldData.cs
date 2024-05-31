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
                stream.Int32(ref WorldDataPtr);
            }
            else
            {
                // We can't serialize the XComWorldDataContainer object in a way the UDK can understand, so we
                // need to remove any reference to it as well
                int data = 0;
                stream.Int32(ref data);
            }
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (XComWorldData) sourceObj;

            WorldDataPtr = Archive.MapIndexFromSourceArchive(other.WorldDataPtr, other.Archive);
        }
    }
}
