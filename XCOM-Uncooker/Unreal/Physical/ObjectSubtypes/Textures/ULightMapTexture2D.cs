using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Actor;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Textures
{
    [Flags]
    public enum ELightMapFlags
    {
        None           = 0,         
        Streamed       = 0x01,
        SimpleLightmap = 0x02
    };

    public class ULightMapTexture2D(FArchive archive, FObjectTableEntry tableEntry) : UTexture2D(archive, tableEntry)
    {
        #region Serialized data

        public ELightMapFlags LightMapFlags;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Enum32(ref LightMapFlags);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (ULightMapTexture2D) sourceObj;

            LightMapFlags = other.LightMapFlags;
        }
    }
}
