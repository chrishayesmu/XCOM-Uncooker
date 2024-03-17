using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.Unreal.Physical
{
    [Flags]
    public enum StructFlag
    {
        Native               = 0x00000001,
        Export               = 0x00000002,
        HasComponents        = 0x00000004,
        Transient            = 0x00000008,

        /// <summary>
        /// If set, then if any member of this struct is serialized, the entire struct is serialized.
        /// </summary>
        Atomic               = 0x00000010,

        /// <summary>
        /// If set, this struct will use binary serialization instead of property tags. Setting <c>Immutable</c> implies setting <c>Atomic</c> also.
        /// </summary>
        Immutable            = 0x00000020,

        StrictConfig         = 0x00000040,

        /// <summary>
        /// If set, this struct will be considered immutable when it is cooked. Setting <c>ImmutableWhenCooked</c> implies setting <c>AtomicWhenCooked</c> also.
        /// </summary>
        ImmutableWhenCooked  = 0x00000080,

        /// <summary>
        /// If set, this struct will be considered atomic when it is cooked.
        /// </summary>
        AtomicWhenCooked     = 0x00000100,
    }

    /// <summary>
    /// Represents a struct type which has been defined in UnrealScript.
    /// </summary>
    public class UScriptStruct(FArchive archive, FObjectTableEntry tableEntry) : UStruct(archive, tableEntry)
    {
        public StructFlag StructFlags;

        /// <summary>
        /// Contains the default values for the properties defined in this struct.
        /// </summary>
        public List<USerializedProperty> StructDefaultProperties = [];

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Enum32(out StructFlags);

            SerializeScriptProperties(StructDefaultProperties, stream);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UScriptStruct other = (UScriptStruct) sourceObj;

            StructFlags = other.StructFlags;

            // TODO clone default properties
        }
    }
}
