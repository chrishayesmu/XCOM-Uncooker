using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical
{
    [Flags]
    public enum ClassFlag : uint
    {
        None = 0x00000000,
        Abstract = 0x00000001,
        Compiled = 0x00000002,
        Config = 0x00000004,
        Transient = 0x00000008,
        Parsed = 0x00000010,
        Localized = 0x00000020,
        SafeReplace = 0x00000040,
        Native = 0x00000080,
        NoExport = 0x00000100,
        Placeable = 0x00000200,
        PerObjectConfig = 0x00000400,
        NativeReplication = 0x00000800,
        EditInlineNew = 0x00001000,
        CollapseCategories = 0x00002000,
        Interface = 0x00004000,
        HasInstancedProps = 0x00200000,
        NeedsDefProps = 0x00400000,
        HasComponents = 0x00800000,
        Hidden = 0x01000000,
        Deprecated = 0x02000000,
        HideDropDown = 0x04000000,
        Exported = 0x08000000,
        Intrinsic = 0x10000000,
        UniqueComponent = 0x20000000,
        PerObjectLocalized = 0x40000000,

        [Obsolete] IsAUProperty = 0x00008000,
        [Obsolete] IsAUObjectProperty = 0x00010000,
        [Obsolete] IsAUBoolProperty = 0x00020000,
        [Obsolete] IsAUState = 0x00040000,
        [Obsolete] IsAUFunction = 0x00080000,
        [Obsolete] IsAUStructProperty = 0x00100000,
    }

    [Flags]
    public enum ClassCastFlag : uint
    {
        None = 0x00000000,
        UField = 0x00000001,
        UConst = 0x00000002,
        UEnum = 0x00000004,
        UStruct = 0x00000008,
        UScriptStruct = 0x00000010,
        UClass = 0x00000020,
        UByteProperty = 0x00000040,
        UIntProperty = 0x00000080,
        UFloatProperty = 0x00000100,
        UComponentProperty = 0x00000200,
        UClassProperty = 0x00000400,
        UInterfaceProperty = 0x00001000,
        UNameProperty = 0x00002000,
        UStrProperty = 0x00004000,
        UProperty = 0x00008000,
        UObjectProperty = 0x00010000,
        UBoolProperty = 0x00020000,
        UState = 0x00040000,
        UFunction = 0x00080000,
        UStructProperty = 0x00100000,
        UArrayProperty = 0x00200000,
        UMapProperty = 0x00400000,
        UDelegateProperty = 0x00800000,
        UComponent = 0x01000000
    }

    public struct FReplicationRecord
    {
        public int Property; // TODO probably not an int
        public int Index;
    }

    public class UClass(FArchive archive, FObjectTableEntry tableEntry) : UState(archive, tableEntry)
    {
        public ClassFlag ClassFlags;

        [Index(typeof(UClass))]
        public int Within;

        public FName ConfigName;
        public IDictionary<FName, int> ComponentNameToDefaultObjectMap;
        public IDictionary<int, int> Interfaces; // first int: index to interface class's definition; second int: UProperty which is some kind of vtable pointer
        public FName[] DontSortCategories;
        public FName[] HideCategories;
        public FName[] AutoExpandCategories;
        public FName[] AutoCollapseCategories;
        public bool ForceScriptOrder;
        public FName[] ClassGroups;
        public string NativeClassName;
        public FName DllBindName;
        
        [Index(typeof(UObject))]
        public int ClassDefaultObject;

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Enum32(ref ClassFlags);
            stream.Int32(ref Within);
            stream.Name(ref ConfigName);
            stream.Map(ref ComponentNameToDefaultObjectMap);
            stream.Map(ref Interfaces);
            stream.NameArray(ref DontSortCategories);
            stream.NameArray(ref HideCategories);
            stream.NameArray(ref AutoExpandCategories);
            stream.NameArray(ref AutoCollapseCategories);
            stream.BoolAsInt32(ref ForceScriptOrder);
            stream.NameArray(ref ClassGroups);
            stream.String(ref NativeClassName);
            stream.Name(ref DllBindName);
            stream.Int32(ref ClassDefaultObject);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UClass other = (UClass) sourceObj;

            ClassFlags = other.ClassFlags;
            Within = Archive.MapIndexFromSourceArchive(other.Within, other.Archive);
            ConfigName = Archive.MapNameFromSourceArchive(other.ConfigName);

            ComponentNameToDefaultObjectMap = new Dictionary<FName, int>(other.ComponentNameToDefaultObjectMap.Count);

            foreach (var entry in other.ComponentNameToDefaultObjectMap)
            {
                FName destKey = Archive.MapNameFromSourceArchive(entry.Key);
                int destValue = Archive.MapIndexFromSourceArchive(entry.Value, other.Archive);

                ComponentNameToDefaultObjectMap.Add(destKey, destValue);
            }

            Interfaces = new Dictionary<int, int>(other.Interfaces.Count);

            foreach (var entry in other.Interfaces)
            {
                int destKey = Archive.MapIndexFromSourceArchive(entry.Key, other.Archive);
                int destValue = Archive.MapIndexFromSourceArchive(entry.Value, other.Archive);

                Interfaces.Add(destKey, destValue);
            }
            
            DontSortCategories = CloneNameArrayFromArchive(other.DontSortCategories);
            HideCategories = CloneNameArrayFromArchive(other.HideCategories);
            AutoExpandCategories = CloneNameArrayFromArchive(other.AutoExpandCategories);
            AutoCollapseCategories = CloneNameArrayFromArchive(other.AutoCollapseCategories);
            ForceScriptOrder = other.ForceScriptOrder;
            ClassGroups = CloneNameArrayFromArchive(other.ClassGroups);
            NativeClassName = other.NativeClassName;
            DllBindName = Archive.MapNameFromSourceArchive(other.DllBindName);
            ClassDefaultObject = Archive.MapIndexFromSourceArchive(other.ClassDefaultObject, other.Archive);
        }
    }

}
