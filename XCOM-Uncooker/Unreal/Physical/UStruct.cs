using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core.Properties;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.Unreal.Physical
{
    public class UStruct(FArchive archive, FObjectTableEntry tableEntry) : UField(archive, tableEntry)
    {
        private static readonly Logger Log = new Logger(nameof(UStruct));

        public UField FirstChild => (UField) Archive.GetObjectByIndex(Children);

        public UField SuperField => (UField) Archive.GetObjectByIndex(Super);

        #region Serialized data

        // The type of UField pointed at here will depend on what type of UStruct this is
        [Index(typeof(UField))]
        public int Super;

        [Index(typeof(UTextBuffer))]
        public int ScriptText;

        [Index(typeof(UTextBuffer))]
        public int CppText;

        /// <summary>
        /// Points to the first child of this struct, which itself contains pointers to the next child, and so on.
        /// </summary>
        [Index(typeof(UField))]
        public int Children;

        public int Line;
        public int TextPos;
        public int ByteScriptSize;
        public int DataScriptSize;

        #endregion

        private List<UProperty> linkedProperties = new List<UProperty>();
        private bool arePropertiesLinked = false;
        private bool hasDeserializedStructData = false;

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(ref Super);
            stream.Int32(ref ScriptText);
            stream.Int32(ref Children);
            stream.Int32(ref CppText);
            stream.Int32(ref Line);
            stream.Int32(ref TextPos);
            stream.Int32(ref ByteScriptSize);
            stream.Int32(ref DataScriptSize);

            // We aren't interested in script data, just skip it
            // TODO we'll need them eventually
            stream.SkipBytes(DataScriptSize);

            hasDeserializedStructData = true;
        }

        /// <summary>
        /// Serializes a property data block using the given properties. XCOM Uncooker can read both normal property
        /// tags and binary-serialized data, but will only write normal tags, since that is what is needed for editor usage.
        /// </summary>
        /// <param name="props">Property list to serialize. When loading, will be appended to. Non-null.</param>
        /// <param name="stream">The data stream to either read from or write to.</param>
        /// <remarks>
        /// For non-binary serialization, the end of the property block is denoted by an <see cref="FPropertyTag"/> which
        /// begins with <c>NAME_None</c> and no other data (so functionally, just the name 'None'). For binary serialization, the end
        /// is implied by knowing how large the block should be.
        /// </remarks>
        public override void SerializeTaggedProperties(List<USerializedProperty> props, IUnrealDataStream stream)
        {
            if (stream.Archive.IsLoading)
            {
                LinkProperties();

                if (!arePropertiesLinked)
                {
                    base.SerializeTaggedProperties(props, stream);
                    return;
                }

                // If this struct doesn't have any properties (maybe it's intrinsic, or a UFunction, etc), fall back on just tag data
                if (linkedProperties.Count == 0)
                {
                    base.SerializeTaggedProperties(props, stream);
                    return;
                }

                FPropertyTag tag = default;
                stream.Object(ref tag);

                while (!tag.Name.IsNone())
                {
                    // Look for the matching property before serializing. They should be in the same order
                    UProperty currentProperty = linkedProperties.FirstOrDefault(p => p.ObjectName == tag.Name);

#if DEBUG
                    if (currentProperty == null)
                    {
                        Log.Error($"{FullObjectPath}: failed to find a property named {tag.Name} to match tag of type {tag.Type}");
                        Debugger.Break();
                        return;
                    }
#endif

                    // Having found our property and tag to match, go ahead and proceed with serialization
                    var prop = currentProperty.CreateSerializedProperty(stream.Archive!, tag);

#if DEBUG
                    // Make sure our property matches the type from the property tag
                    if (prop.TagType != tag.Type)
                    {
                        Log.Error($"{FullObjectPath}: found a property named {tag.Name}, but the tag type {tag.Type} didn't match the property's tag type {prop.TagType}. Property type is {currentProperty.GetType()}");
                        return;
                    }
#endif

                    prop.Serialize(stream);
                    props.Add(prop);

                    // Move on to the next property tag
                    stream.Object(ref tag);
                }
            }
            else
            {
                for (int i = 0; i < props.Count; i++)
                {
                    if (props[i].Tag != null)
                    {
                        var tag = props[i].Tag!.Value;
                        stream.Object(ref tag);
                    }

                    props[i].Serialize(stream);
                }

                FName nameNone = stream.Archive.GetOrCreateName("None");
                stream.Name(ref nameNone);
            }
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UStruct other = (UStruct) sourceObj;

            Super = Archive.MapIndexFromSourceArchive(other.Super, other.Archive);
            ScriptText = Archive.MapIndexFromSourceArchive(other.ScriptText, other.Archive);
            Children = Archive.MapIndexFromSourceArchive(other.Children, other.Archive);
            CppText = Archive.MapIndexFromSourceArchive(other.CppText, other.Archive);
            Line = other.Line;
            TextPos = other.TextPos;
            ByteScriptSize = other.ByteScriptSize;
            DataScriptSize = other.DataScriptSize;
        }

        public UProperty GetPropertyByName(string name)
        {
            UField current = FirstChild;

            while (current != null)
            {
                if (current is UProperty prop && current.ObjectName == name)
                {
                    return prop;
                }

                current = current.NextField;
            }

            return null;
        }

        private UProperty GetNextProperty(UField field) 
        {
            UField current = field;

            while (current != null && current is not UProperty)
            {
                current = current.NextField;
            }

            return (UProperty) current;
        }

        private void LinkProperties()
        {
            lock (this)
            {
                // Don't try to link properties until we've had a chance to read the struct's data, or we 
                // won't find anything. This should only apply when deserializing an archive.
                if (arePropertiesLinked || (!hasDeserializedStructData && Archive.IsLoading))
                {
                    return;
                }

                arePropertiesLinked = true;
           
                // Go through the inheritance hierarchy from top to bottom
                var inheritanceHierarchy = new Stack<UStruct>();
            
                UStruct structDef = this;
                while (structDef != null)
                {
                    inheritanceHierarchy.Push(structDef);
                    structDef = (UStruct) structDef.SuperField;
                }

                // Add property fields in order as we find them
                while (inheritanceHierarchy.TryPop(out structDef))
                {
                    UProperty prop = structDef.GetNextProperty(structDef.FirstChild);

                    while (prop != null)
                    {
                        linkedProperties.Add(prop);
                        prop = structDef.GetNextProperty(prop.NextField);
                    }
                }
            }
        }
    }
}
