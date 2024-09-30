using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Actor;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.Textures
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

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UTextureMovie) sourceObj;

            Data = other.Data;
        }
    }
}
