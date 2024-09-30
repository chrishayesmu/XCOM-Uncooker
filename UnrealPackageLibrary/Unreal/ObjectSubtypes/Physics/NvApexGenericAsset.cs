using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Actor;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.Physics
{
    public class NvApexGenericAsset(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public bool bAssetValid; // UBOOL

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.BoolAsInt32(ref bAssetValid);

            // There would be more data here if bAssetValid == true, but that never seems to apply in XCOM EW
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (NvApexGenericAsset) sourceObj;

            bAssetValid = other.bAssetValid;
        }
    }
}
