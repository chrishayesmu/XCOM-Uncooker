using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;

namespace UnrealArchiveLibrary.Unreal
{
    [Flags]
    public enum FunctionFlag
    {
        Final            = 0x00000001,
        Defined          = 0x00000002,
        Iterator         = 0x00000004,
        Latent           = 0x00000008,
        PreOperator      = 0x00000010,
        Singular         = 0x00000020,
        Net              = 0x00000040,
        NetReliable      = 0x00000080,
        Simulated        = 0x00000100,
        Exec             = 0x00000200,
        Native           = 0x00000400,
        Event            = 0x00000800,
        Operator         = 0x00001000,
        Static           = 0x00002000,
        HasOptionalParms = 0x00004000,
        Const            = 0x00008000,
        // Gap 
        Public           = 0x00020000,
        Private          = 0x00040000,
        Protected        = 0x00080000,
        Delegate         = 0x00100000,
        NetServer        = 0x00200000,
        HasOutParms      = 0x00400000,
        HasDefaults      = 0x00800000,
        NetClient        = 0x01000000
    }

    public class UFunction(FArchive archive, FObjectTableEntry tableEntry) : UStruct(archive, tableEntry)
    {
        public short NativeIndex;
        public byte OperatorPrecedence;
        public FunctionFlag FunctionFlags;
        public short RepOffset;
        public FName FriendlyName;

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int16(ref NativeIndex);
            stream.UInt8(ref OperatorPrecedence);
            stream.Enum32(ref FunctionFlags);

            if (FunctionFlags.HasFlag(FunctionFlag.Net))
            {
                stream.Int16(ref RepOffset);
            }

            stream.Name(ref FriendlyName);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UFunction other = (UFunction) sourceObj;

            NativeIndex = other.NativeIndex;
            OperatorPrecedence = other.OperatorPrecedence;
            FunctionFlags = other.FunctionFlags;
            RepOffset = other.RepOffset;
            FriendlyName = Archive.MapNameFromSourceArchive(other.FriendlyName);
        }
    }
}
