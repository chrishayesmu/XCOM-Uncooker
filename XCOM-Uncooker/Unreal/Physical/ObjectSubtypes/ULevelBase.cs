using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes
{
    public class ULevelBase(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        [Index(typeof(UObject))]
        public int Actors_OwnerIndex;

        [Index(typeof(UObject))]
        public int[] Actors_Data;

        public FURL Url = new FURL();

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(out Actors_OwnerIndex);
            stream.Int32Array(out Actors_Data);
            Url.Serialize(stream);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            ULevelBase other = (ULevelBase)sourceObj;

            Actors_OwnerIndex = Archive.MapIndexFromSourceArchive(other.Actors_OwnerIndex, other.Archive);

            Actors_Data = new int[other.Actors_Data.Length];

            for (int i = 0; i < Actors_Data.Length; i++)
            {
                Actors_Data[i] = Archive.MapIndexFromSourceArchive(other.Actors_Data[i], other.Archive);
            }

            Url = other.Url;
        }
    }
}
