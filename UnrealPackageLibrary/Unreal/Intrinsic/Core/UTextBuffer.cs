using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;

namespace UnrealArchiveLibrary.Unreal
{
    /// <summary>
    /// A text buffer which is a standalone object, rather than being a string written inline
    /// into an object's properties or other data regions.
    /// </summary>
    public class UTextBuffer(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        public int Pos;
        public int Top;
        public string Text;

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(ref Pos);
            stream.Int32(ref Top);
            stream.String(ref Text);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UTextBuffer other = (UTextBuffer) sourceObj;

            Pos = other.Pos;
            Top = other.Top;
            Text = other.Text;
        }
    }
}
