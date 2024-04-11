using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.Intrinsic.Core;

namespace XCOM_Uncooker.Unreal.Physical
{
    /// <summary>
    /// Abstract representation of an entry in either the import or export table of an <see cref="FArchive"/>.
    /// While the internal data of these types in an archive is fairly different, they share an important responsibility:
    /// identifying an object's class and owner. This is needed very often when parsing an archive, therefore
    /// the two types provide a common abstraction to simplify parsing withref having to always worry abref whether
    /// an object is an important or an export.
    /// </summary>
    public abstract class FObjectTableEntry
    {
        /// <summary>
        /// The file archive which contains this table entry. If this entry is an import,
        /// then this is <b>not</b> the same archive where the object itself can be found.
        /// </summary>
        public FArchive Archive;

        /// <summary>
        /// The class name of the object for this entry.
        /// </summary>
        public abstract FName ClassName { get; }

        /// <summary>
        /// The class name of the object for this entry, in string form.
        /// </summary>
        public string ClassNameString => Archive.NameToString(ClassName);

        /// <summary>
        /// The name of the object which this entry references.
        /// </summary>
        public FName ObjectName;

        /// <summary>
        /// The index of the object which owns the object that this entry references. For an export object, the
        /// outer object must always be another export; for an import object, it must be another import. In either
        /// case, the OuterIndex may be 0, indicating the object is top-level and has no outer object.
        /// </summary>
        public int OuterIndex;

        /// <summary>
        /// Whether this object is a class definition.
        /// </summary>
        public abstract bool IsClass { get; }

        /// <summary>
        /// Whether this object is a package definition.
        /// </summary>
        public abstract bool IsPackage { get; }

        /// <summary>
        /// The UClass object which defines this object's class.
        /// </summary>
        public abstract UClass ClassObj { get; }

        /// <summary>
        /// Gets this object's Outer object, which can be considered its owner in a sense. This may be a data object,
        /// a package, a Class Default Object, etc. Top-level objects do not have an Outer and will return null. Examples
        /// of top-level objects include a package, or in map archives, an object called "TheWorld".
        /// </summary>
        public UObject Outer => Archive.GetObjectByIndex(OuterIndex);

        /// <summary>
        /// Gets the table entry for the outermost object containing this object.
        /// </summary>
        public FObjectTableEntry Outermost
        {
            get
            {
                var outermost = this;

                while (outermost.OuterIndex != 0)
                {
                    outermost = outermost.Archive.GetObjectTableEntry(outermost.OuterIndex);
                }

                return outermost;
            }
        }

        /// <summary>
        /// Similar to <see cref="Outer"/>, but only pulls the table entry for the Outer object instead. Faster and
        /// potentially safer to use if only table data is needed, and the full object may not be available (e.g. it
        /// is imported from an archive file that hasn't been loaded).
        /// </summary>
        public FObjectTableEntry OuterTable => Archive.GetObjectTableEntry(OuterIndex);

        /// <summary>
        /// The complete path which identifies this object, such as <c>MyPkg.ClassName</c>, or <c>TheWorld.PersistentLevel.SomeObject.ChildObject</c>.
        /// </summary>
        public string FullObjectPath
        {
            get
            {
                if (_cachedFullObjectPath != "")
                {
                    return _cachedFullObjectPath;
                }

                var pathBuilder = new StringBuilder(ObjectName);

                var outerTable = OuterTable;
                FObjectTableEntry lastOuter = null;

                while (outerTable != null)
                {
                    pathBuilder.Insert(0, ".");
                    pathBuilder.Insert(0, outerTable.ObjectName);

                    lastOuter = outerTable;
                    outerTable = Archive.GetObjectTableEntry(outerTable.OuterIndex);
                }

                string path = pathBuilder.ToString();

                if (lastOuter != null)
                {
                    string topLevelPackage = Archive.ParentLinker.GetTopLevelPackageName(lastOuter) + ".";

                    if (!path.StartsWith(topLevelPackage))
                    {
                        path= topLevelPackage + path;
                    }
                }
                else if (!IsPackage)
                {
                    path = Archive.NormalizedName + "." + path;
                }

                _cachedFullObjectPath = path;
                return path;
            }
        }

        private string _cachedFullObjectPath = "";

        /// <summary>
        /// The 0-based index of this table entry into its archive's corresponding table, either <see cref="FArchive.ExportTable"/> or 
        /// <see cref="FArchive.ImportTable"/>. This is exposed for convenience and performance.
        /// </summary>
        public int TableEntryIndex;

        /// <summary>
        /// Serializes or deserializes the table entry. Assumes that the stream's current position is at the start of the entry.
        /// </summary>
        public abstract void Serialize(IUnrealDataStream stream);
    }
}
