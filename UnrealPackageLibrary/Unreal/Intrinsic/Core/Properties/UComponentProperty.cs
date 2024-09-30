using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;

namespace UnrealArchiveLibrary.Unreal.Intrinsic.Core.Properties
{
    public class UComponentProperty(FArchive archive, FObjectTableEntry tableEntry) : UObjectProperty(archive, tableEntry)
    {
        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);
        }
    }
}
