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
    public enum StateFlag
    {
        Editable  = 0x00000001,
        Auto      = 0x00000002,
        Simulated = 0x00000004
    };

    public class UState(FArchive archive, FObjectTableEntry tableEntry) : UStruct(archive, tableEntry)
    {
        public int ProbeMask;
        public short LabelTableOffset;
        public StateFlag StateFlags;

        [Index(typeof(UFunction))] // Dictionary value is an index
        public IDictionary<FName, int> FuncMap;

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Int32(ref ProbeMask);
            stream.Int16(ref LabelTableOffset);
            stream.Enum32(ref StateFlags);
            stream.Map(ref FuncMap);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            UState other = (UState) sourceObj;

            ProbeMask = other.ProbeMask;
            LabelTableOffset = other.LabelTableOffset;
            StateFlags = other.StateFlags;

            FuncMap = new Dictionary<FName, int>(other.FuncMap.Count);

            foreach (var entry in other.FuncMap)
            {
                var destKey = Archive.MapNameFromSourceArchive(entry.Key);
                var destValue = Archive.MapIndexFromSourceArchive(entry.Value, other.Archive);

                FuncMap.Add(destKey, destValue);
            }
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            dependencyIndices.AddRange(FuncMap.Values);
        }
    }
}
