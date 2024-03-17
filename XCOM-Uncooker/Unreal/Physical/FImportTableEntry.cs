using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core;

namespace XCOM_Uncooker.Unreal.Physical
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public class FImportTableEntry: FObjectTableEntry
    {
        public FName ClassPackage;

        public FName _className;
        public override FName ClassName => _className;

        public override bool IsClass => _className == "Class";

        public override bool IsPackage => ClassPackage == "Core" && ClassName == "Package";

        // TODO do we need to bother implementing this for uncooked archives?
        public override UClass ClassObj => Archive.IsCooked ? (UClass) Archive.ParentLinker.GetCookedObjectByPath($"{ClassPackage}.{ClassName}") : null;

        public FImportTableEntry(FArchive archive)
        {
            this.Archive = archive;
        }

        public override void Serialize(IUnrealDataStream stream)
        {
            stream.Name(out ClassPackage);
            stream.Name(out _className);
            stream.Int32(out OuterIndex);
            stream.Name(out ObjectName);
        }

        private string DebuggerDisplay
        { 
            get
            {
                return $"{ClassPackage}.{ClassName}: {ObjectName} - OuterIndex {OuterIndex}";
            }
        }
    }
}
