using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.Materials
{
    public class FStaticParameterSet : IUnrealSerializable
    {
        public Guid BaseMaterialId;

        public FStaticSwitchParameter[] StaticSwitchParameters;

        public FStaticComponentMaskParameter[] StaticComponentMaskParameters;
        
        public FNormalParameter[] NormalParameters;

        public FStaticTerrainLayerWeightParameter[] TerrainLayerWeightParameters;

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Guid(ref BaseMaterialId);
            stream.Array(ref StaticSwitchParameters);
            stream.Array(ref StaticComponentMaskParameters);
            stream.Array(ref NormalParameters);
            stream.Array(ref TerrainLayerWeightParameters);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FStaticParameterSet) sourceObj;

            BaseMaterialId = other.BaseMaterialId;
            StaticSwitchParameters = IUnrealSerializable.Clone(other.StaticSwitchParameters, sourceArchive, destArchive);
            StaticComponentMaskParameters = IUnrealSerializable.Clone(other.StaticComponentMaskParameters, sourceArchive, destArchive);
            NormalParameters = IUnrealSerializable.Clone(other.NormalParameters, sourceArchive, destArchive);
            TerrainLayerWeightParameters = IUnrealSerializable.Clone(other.TerrainLayerWeightParameters, sourceArchive, destArchive);
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }
}
