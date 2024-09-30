using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Actor;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.Textures
{
    public class UTexture(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public FUntypedBulkData SourceArt = new FUntypedBulkData();

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            SourceArt.Serialize(stream);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UTexture) sourceObj;

            SourceArt.CloneFromOtherArchive(other.SourceArt, other.Archive, Archive);
        }
    }
}
