using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Actor;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Physics;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Components
{
    public class UBrushComponent(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public FKCachedConvexData CachedPhysBrushData;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Object(ref CachedPhysBrushData);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UBrushComponent) sourceObj;

            CachedPhysBrushData = other.CachedPhysBrushData;
        }
    }
}
