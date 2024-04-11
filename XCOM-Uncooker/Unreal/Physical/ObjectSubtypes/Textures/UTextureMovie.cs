using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Textures
{
    public class UTextureMovie(FArchive archive, FObjectTableEntry tableEntry) : UTexture(archive, tableEntry)
    {
        #region Serialized data

        public FUntypedBulkData Data = new FUntypedBulkData();

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            Data.Serialize(stream);
        }
    }
}
