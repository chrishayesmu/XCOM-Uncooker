using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Actor;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Components;
using UnrealArchiveLibrary.Unreal.ObjectSubtypes.Lighting;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.Physics
{
    public class UFluidSurfaceComponent(FArchive archive, FObjectTableEntry tableEntry) : UObject(archive, tableEntry)
    {
        #region Serialized data

        public FLightMap LightMap;

        #endregion

        public override void Serialize(IUnrealDataStream stream)
        {
            base.Serialize(stream);

            stream.Object(ref LightMap);
        }

        public override void CloneFromOtherArchive(UObject sourceObj)
        {
            base.CloneFromOtherArchive(sourceObj);

            var other = (UFluidSurfaceComponent) sourceObj;

            LightMap = new FLightMap();
            LightMap.CloneFromOtherArchive(other.LightMap, other.Archive, Archive);
        }

        public override void PopulateDependencies(List<int> dependencyIndices)
        {
            base.PopulateDependencies(dependencyIndices);

            LightMap.PopulateDependencies(dependencyIndices);
        }
    }
}
