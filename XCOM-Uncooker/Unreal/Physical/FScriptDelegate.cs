using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical.SerializedProperties;

namespace XCOM_Uncooker.Unreal.Physical
{
    public class FScriptDelegate(FArchive archive)
    {
        public FArchive Archive { get; private set; } = archive;

        #region Serialized data

        [Index(typeof(UObject))]
        public int ObjectIndex;

        public FName FunctionName;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Int32(out ObjectIndex);
            stream.Name(out FunctionName);
        }

        public void CloneFromOtherArchive(FScriptDelegate other)
        {
            ObjectIndex = Archive.MapIndexFromSourceArchive(other.ObjectIndex, other.Archive);
            FunctionName = Archive.MapNameFromSourceArchive(other.FunctionName);
        }
    }
}
