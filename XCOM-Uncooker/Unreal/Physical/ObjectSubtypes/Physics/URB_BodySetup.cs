using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Actor;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Physics
{
    public class URB_BodySetup(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public FKCachedConvexData PreCachedPhysData;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Object(ref PreCachedPhysData);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (URB_BodySetup) sourceObj;

            PreCachedPhysData = other.PreCachedPhysData;
        }
    }
}
