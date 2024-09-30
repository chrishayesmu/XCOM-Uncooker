using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnrealArchiveLibrary.Unreal
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field | AttributeTargets.Property)]
    public class FixedSizeAttribute(int size) : Attribute
    {
        public int Size { get; } = size;
    }
}
