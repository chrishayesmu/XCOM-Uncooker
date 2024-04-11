using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Level
{
    /// <summary>
    /// This struct just exists to be used as a generic type for TTransactionalArray.
    /// </summary>
    public struct ActorPointer : IUnrealSerializable
    {
        #region Serialized data
        
        public int Index;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(out Index);
        }
    }

    public class ULevelBase(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        [Index(typeof(UObject))]
        public TTransactionalArray<ActorPointer> Actors;

        public FURL Url;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Object(out Actors);
            stream.Object(out Url);
        }
    }
}
