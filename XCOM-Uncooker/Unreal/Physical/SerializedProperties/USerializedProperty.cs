using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;

namespace XCOM_Uncooker.Unreal.Physical.SerializedProperties
{
    /// <summary>
    /// Serialized properties are preceded by an FPropertyTag, which provides the
    /// necessary metadata to deserialize the property.
    /// </summary>
    public struct FPropertyTag(FArchive Archive) : IUnrealSerializable
    {
        /// <summary>
        /// The name of the property, which will match the name of a corresponding <see cref="UProperty"/>
        /// in the type which this property tag is part of.
        /// </summary>
        public FName Name;

        /// <summary>
        /// The name of the type of the property, which generally should match the type defined in the corresponding
        /// <see cref="UProperty"/>, but may not if the property's definition has changed since this object was serialized.
        /// </summary>
        public FName Type;

        /// <summary>
        /// The size of the property data, not including the tag. The data block is located
        /// immediately after the end of the property tag.
        /// </summary>
        public int Size;

        /// <summary>
        /// If the property data this tag is associated with is part of a static array, then this
        /// is the index of the data in that array. Otherwise, this will be 0.
        /// </summary>
        public int ArrayIndex;

        /// <summary>
        /// Only set if Type == "ByteProperty". Name of the enum type stored in this property.
        /// </summary>
        public FName EnumName;

        /// <summary>
        /// Only set if Type == "StructProperty". Name of the struct's type.
        /// </summary>
        public FName StructName;

        /// <summary>
        /// Only set if Type == "BoolProperty". The boolean value is stored here, and the
        /// property body is not needed.
        /// </summary>
        public bool BoolVal;

        public readonly bool IsBoolProperty => Type == "BoolProperty";
        public readonly bool IsEnumProperty => Type == "EnumProperty";
        public readonly bool IsStructProperty => Type == "StructProperty";

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Name(ref Name);

            if (Name.IsNone())
            {
                return;
            }

            stream.Name(ref Type);
            stream.Int32(ref Size);
            stream.Int32(ref ArrayIndex);

            if (Type == "BoolProperty")
            {
                stream.Bool(ref BoolVal);
            }
            else if (Type == "ByteProperty")
            {
                stream.Name(ref EnumName);
            }
            else if (Type == "StructProperty")
            {
                stream.Name(ref StructName);
            }
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FPropertyTag) sourceObj;

            Name = destArchive.MapNameFromSourceArchive(other.Name);
            Type = destArchive.MapNameFromSourceArchive(other.Type);
            Size = other.Size;
            ArrayIndex = other.ArrayIndex;
            EnumName = destArchive.MapNameFromSourceArchive(other.EnumName);
            StructName = destArchive.MapNameFromSourceArchive(other.StructName);
            BoolVal = other.BoolVal;
        }
    }

    /// <summary>
    /// Represents a property which has been serialized into an <see cref="FArchive"/>. Serialized properties
    /// may be nested, such as in <c>array&lt;T&gt;</c> or <c>var T array[size]</c> declarations. Each top-level
    /// serialized property has a corresponding <see cref="UProperty"/> in its owning data type.
    /// </summary>
    /// <remarks>
    /// Generally, a property will be serialized only if it differs from the value set in the object's archetype, 
    /// though not always. For example, UnrealScript structs marked with <c>atomic</c> will serialize all of their 
    /// properties if any of them get serialized.
    /// </remarks>
    public abstract class USerializedProperty(FArchive archive, UProperty prop, FPropertyTag? tag)
    {
        /// <summary>
        /// The value which should appear in <see cref="FPropertyTag.Type"/> in order for this property type
        /// to be used. Mainly used for debugging.
        /// </summary>
        public abstract string TagType { get; }

        /// <summary>
        /// The <see cref="UProperty"/> which defines this property in a <see cref="UStruct"/> somewhere.
        /// </summary>
        public UProperty BackingProperty { get; private set; } = prop;

        public FArchive Archive { get; private set; } = archive;

        /// <summary>
        /// Whether this property contains any data. Each block of serialized properties is ended by a property with
        /// an empty tag name, so there are some serialized properties which do not have data.
        /// </summary>
        public bool HasData => !Tag?.Name.IsNone() ?? true;

        #region Serialized data

        /// <summary>
        /// The property's tag, which is largely metadata about the property and how to interpret it.
        /// Not all serialized properties have tags; in particular, immutable structs and items serialized
        /// inside of arrays won't have tags.
        /// </summary>
        public FPropertyTag? Tag { get; private set; } = tag;

        #endregion

        public abstract void Serialize(IUnrealDataStream stream);

        public virtual void CloneFromOtherArchive(USerializedProperty sourceProp)
        {
            if (sourceProp.Tag != null)
            {
                Tag = new FPropertyTag();
                Tag.Value.CloneFromOtherArchive(sourceProp.Tag.Value, sourceProp.Archive, Archive);
            }
        }
    }
}
