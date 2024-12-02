using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.SerializedProperties;
using UnrealPackageLibrary;

namespace UnrealArchiveLibrary.Unreal
{
    public class FScriptDelegate(FArchive archive)
    {
        public FArchive Archive { get; private set; } = archive;

        #region Serialized data

        [Index(typeof(UObject))]
        public int ObjectIndex;

        public FName FunctionName;

        public int XCom2_Unknown;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(ref ObjectIndex);
            stream.Name(ref FunctionName);

            if (Archive.Format == ArchiveFormat.XCom2WotC)
            {
                stream.Int32(ref XCom2_Unknown);
            }
        }

        public void CloneFromOtherArchive(FScriptDelegate other)
        {
            ObjectIndex = Archive.MapIndexFromSourceArchive(other.ObjectIndex, other.Archive);
            FunctionName = Archive.MapNameFromSourceArchive(other.FunctionName);
            XCom2_Unknown = other.XCom2_Unknown;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
            dependencyIndices.Add(ObjectIndex);
        }
    }
}
