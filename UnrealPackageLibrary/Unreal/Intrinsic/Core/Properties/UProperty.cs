using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.Intrinsic.Core;
using UnrealArchiveLibrary.Unreal.SerializedProperties;
using UnrealPackageLibrary;

namespace UnrealArchiveLibrary.Unreal.Intrinsic.Core.Properties
{
    /// <summary>
    /// TODO, also convert these to hex and not shifts for uniformity
    /// </summary>
    [Flags]
    public enum PropertyFlag : ulong
    {
        /// <summary>
        /// Indicates that this property can be modified in the editor.
        /// </summary>
        Edit = 1L << 0,

        /// <summary>
        /// 
        /// </summary>
        Const = 1L << 1,
        Input = 1L << 2,
        ExportObject = 1L << 3,
        OptionalParam = 1L << 4,
        Net = 1L << 5,
        EditFixedSize = 1L << 6,
        Param = 1L << 7,
        OutParam = 1L << 8,
        SkipParam = 1L << 9,
        ReturnParam = 1L << 10,
        CoerceParam = 1L << 11,
        Native = 1L << 12,
        Transient = 1L << 13,
        Config = 1L << 14,
        // deliberate gap
        Localized = 1L << 16,
        EditConst = 1L << 17,
        GlobalConfig = 1L << 18,
        Component = 1L << 19,
        AlwaysInit = 1L << 20,
        DuplicateTransient = 1L << 21,
        NeedCtorLink = 1L << 22,
        NoExport = 1L << 23,
        NoImport = 1L << 24,
        NoClear = 1L << 25,
        EditInline = 1L << 26,
        // deliberate gap
        EditInlineUse = 1L << 28,
        Deprecated = 1L << 29,
        DataBinding = 1L << 30,
        SerializeText = 1L << 31,
        RepNotify = 1L << 32,
        Interp = 1L << 33,
        NonTransactional = 1L << 34,
        EditorOnly = 1L << 35,
        NotForConsole = 1L << 36,
        RepRetry = 1L << 37,
        PrivateWrite = 1L << 38,
        ProtectedWrite = 1L << 39,
        ArchetypeProperty = 1L << 40,
        EditHide = 1L << 41,
        EditTextBox = 1L << 42
    }

    /// <summary>
    /// UProperty represents a property of a type, such as a class or a struct. It should not be confused with
    /// the object properties which are serialized in a <see cref="UObject"/>'s data block.
    /// </summary>
    public abstract class UProperty(FArchive archive, FObjectTableEntry tableEntry) : UField(archive, tableEntry)
    {
        #region Serialized data

        /// <summary>
        /// How large is the static array represented by this property. A value of 1 means this is a normal, non-array
        /// property; a value greater than 1 gives the size of the static array.
        /// </summary>
        public int ArrayDim;

        /// <summary>
        /// Flags applied to this property as part of its declaration.
        /// </summary>
        public PropertyFlag PropertyFlags;

        /// <summary>
        /// XCOM 2 only: allows setting a config file for individual fields of a class, in addition to
        /// the setting at the class level.
        /// </summary>
        public FName? ConfigName;

        /// <summary>
        /// What category to display this property under in the Unreal Editor.
        /// </summary>
        public FName CategoryName;

        /// <summary>
        /// If this property is a static array, and its size was set using an enum, then this is the index of the enum;
        /// otherwise, this is 0. For example, this static array declaration:
        /// 
        /// <code>var string ItemNames[EItemType];</code>
        /// 
        /// would result in a property with the index of <c>EitemType</c> in the <c>ArraySizeEnum</c> field.
        /// </summary>
        [Index(typeof(UEnum))]
        public int ArraySizeEnum;

        /// <summary>
        /// Some form of offset for properties which are replicated between client and server; not relevant to this project,
        /// but stored because it's part of the serialized data.
        /// </summary>
        public short ReplicationOffset;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(ref ArrayDim);
            stream.Enum64(ref PropertyFlags);

            if (Archive.Format == ArchiveFormat.XCom2WotC)
            {
                stream.Name(ref ConfigName);
            }

            stream.Name(ref CategoryName);
            stream.Int32(ref ArraySizeEnum);

            if (PropertyFlags.HasFlag(PropertyFlag.Net))
            {
                stream.Int16(ref ReplicationOffset);
            }
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UProperty other = (UProperty)sourceObj;

            ArrayDim = other.ArrayDim;
            PropertyFlags = other.PropertyFlags;
            ConfigName = Archive.MapNameFromSourceArchive(other.ConfigName);
            CategoryName = Archive.MapNameFromSourceArchive(other.CategoryName);
            ArraySizeEnum = Archive.MapIndexFromSourceArchive(other.ArraySizeEnum, other.Archive);
            ReplicationOffset = other.ReplicationOffset;
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            dependencyIndices.Add(ArraySizeEnum);
        }

        protected override void SerializeScriptProperties(List<USerializedProperty> props, IUnrealDataStream stream)
        {
            // Properties never have their own properties, they just immediately terminate
            FName NAME_None = Archive.GetOrCreateName("None");
            stream.Name(ref NAME_None);
        }

        /// <summary>
        /// Creates and returns an instance of the serialized property type corresponding to this property definition.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public abstract USerializedProperty CreateSerializedProperty(FArchive archive, FPropertyTag? tag);
    }
}
