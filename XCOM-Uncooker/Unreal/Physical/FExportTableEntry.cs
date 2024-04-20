using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core;

namespace XCOM_Uncooker.Unreal.Physical
{
    [Flags]
    public enum ExportFlag
    {
        None                    = 0x00000000,
        ForcedExport            = 0x00000001,
        ScriptPatcherExport     = 0x00000002,
        MemberFieldPatchPending = 0x00000004
    }

    [Flags]
    public enum ObjectFlag : ulong
    {
        InSingularFunc         = 0x0000000000000002,
        StateChanged           = 0x0000000000000004,
        DebugPostLoad          = 0x0000000000000008,
        DebugSerialize         = 0x0000000000000010,
        DebugFinishDestroyed   = 0x0000000000000020,
        EdSelected             = 0x0000000000000040,
        ZombieComponent        = 0x0000000000000080,
        Protected              = 0x0000000000000100,
        ClassDefaultObject     = 0x0000000000000200,
        ArchetypeObject        = 0x0000000000000400,
        ForceTagExp            = 0x0000000000000800,
        TokenStreamAssembled   = 0x0000000000001000,
        MisalignedObject       = 0x0000000000002000,
        RootSet                = 0x0000000000004000,
        BeginDestroyed         = 0x0000000000008000,
        FinishDestroyed        = 0x0000000000010000,
        DebugBeginDestroyed    = 0x0000000000020000,
        MarkedByCooker         = 0x0000000000040000,
        LocalizedResource      = 0x0000000000080000,
        InitializedProps       = 0x0000000000100000,
        PendingFieldPatches    = 0x0000000000200000,
        Saved                  = 0x0000000080000000,
        Transactional          = 0x0000000100000000,
        Unreachable            = 0x0000000200000000,
        Public                 = 0x0000000400000000,
        TagImp                 = 0x0000000800000000,
        TagExp                 = 0x0000001000000000,
        Obsolete               = 0x0000002000000000,
        TagGarbage             = 0x0000004000000000,
        DisregardForGC         = 0x0000008000000000,
        PerObjectLocalized     = 0x0000010000000000,
        NeedLoad               = 0x0000020000000000,
        AsyncLoading           = 0x0000040000000000,
        NeedPostLoadSubobjects = 0x0000080000000000,
        Suppress               = 0x0000100000000000,
        InEndState             = 0x0000200000000000,
        Transient              = 0x0000400000000000,
        Cooked                 = 0x0000800000000000,
        LoadForClient          = 0x0001000000000000,
        LoadForServer          = 0x0002000000000000,
        LoadForEdit            = 0x0004000000000000,
        Standalone             = 0x0008000000000000,
        NotForClient           = 0x0010000000000000,
        NotForServer           = 0x0020000000000000,
        NotForEdit             = 0x0040000000000000,
        NeedPostLoad           = 0x0100000000000000,
        HasStack               = 0x0200000000000000,
        Native                 = 0x0400000000000000,
        Marked                 = 0x0800000000000000,
        ErrorShutdown          = 0x1000000000000000,
        PendingKill            = 0x2000000000000000
    }

    public class FExportTableEntry : FObjectTableEntry
    {
        public int ClassIndex;
        public int SuperIndex;
        public int ArchetypeIndex;
        public ObjectFlag ObjectFlags;
        public int SerialSize;
        public int SerialOffset;
        public ExportFlag ExportFlags;
        public int[] GenerationNetObjectCount;
        public Guid PackageGuid;
        public PackageFlag PackageFlags;

        public override FName ClassName => IsClass ? ObjectName : Archive.GetObjectTableEntry(ClassIndex).ObjectName;

        public override bool IsClass => ClassIndex == 0;

        public override bool IsPackage => ClassName == "Package";

        public override UClass ClassObj => (UClass) Archive.GetObjectByIndex(ClassIndex);

        public UClass SuperClassObj => (UClass) Archive.GetObjectByIndex(SuperIndex);

        /// <summary>
        /// The ending position in the archive when this object has been fully serialized. In other words, this is the
        /// position such that the last byte of this object's data is immediately before this position, and the bytes 
        /// beginning at <see cref="SerialEndPosition" /> belong to a different object.
        /// </summary>
        public int SerialEndPosition => SerialOffset + SerialSize;

        public bool IsClassDefaultObject { get; private set; } = false;

        /// <summary>
        /// The exported object associated with this entry. May be null if it hasn't been loaded into the archive yet.
        /// </summary>
        public UObject ExportObject;

        public FExportTableEntry(FArchive archive)
        {
            this.Archive = archive;
        }

        public override void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref ClassIndex);
            stream.Int32(ref SuperIndex);
            stream.Int32(ref OuterIndex);
            stream.Name(ref ObjectName);
            stream.Int32(ref ArchetypeIndex);
            stream.Enum64(ref ObjectFlags);
            stream.Int32(ref SerialSize);
            stream.Int32(ref SerialOffset);
            stream.Enum32(ref ExportFlags);
            stream.Int32Array(ref GenerationNetObjectCount);
            stream.Guid(ref PackageGuid);
            stream.Enum32(ref PackageFlags);

            if (stream.IsRead)
            {
                IsClassDefaultObject = ObjectName.ToString().StartsWith("Default__");
            }
        }

        public override string ToString()
        {
            return $"""
                {nameof(FExportTableEntry)}=(
                   {ObjectName}: {ClassName}
                   ClassIndex={ClassIndex}, SuperIndex={SuperIndex}, OuterIndex={OuterIndex}, ArchetypeIndex={ArchetypeIndex}
                )
                """;
        }
    }
}
