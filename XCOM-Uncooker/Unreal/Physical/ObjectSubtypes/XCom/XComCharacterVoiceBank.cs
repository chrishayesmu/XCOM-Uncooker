using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.XCom
{
    public class XComCharacterVoiceBank(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        // Maps from XComGame.XGGameData.ECharacterSpeech to the corresponding UProperty within the
        // XComCharacterVoiceBank class. Why this exists is uncertain, since custom voice packs work fine
        // withref this data; probably it's a simple optimization.
        [Index(typeof(UProperty))]
        public IDictionary<byte, int> EventToPropertyMap;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            if (!IsClassDefaultObject())
            {
                stream.Map(ref EventToPropertyMap);
            }
        }
    }
}
