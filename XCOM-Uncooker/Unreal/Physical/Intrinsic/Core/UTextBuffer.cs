using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical
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

            stream.Int32(out Pos);
            stream.Int32(out Top);
            stream.String(out Text);
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
