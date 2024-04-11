using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Physics
{
    public class NvApexGenericAsset(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public bool bAssetValid; // UBOOL

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.BoolAsInt32(out bAssetValid);

            // TODO: there could be more data here if bAssetValid == true, but that may never apply in XCOM EW
        }
    }
}
