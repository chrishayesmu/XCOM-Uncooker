using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.IO;

namespace UnrealArchiveLibrary.Unreal.ObjectSubtypes.Materials
{
    public class FStaticComponentMaskParameter : IUnrealSerializable
    {
        #region Serialized data
        
        public FName ParameterName;

        public bool R, G, B, A;

        public bool Override;

        public Guid ExpressionGuid;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Name(ref ParameterName);
            stream.BoolAsInt32(ref R);
            stream.BoolAsInt32(ref G);
            stream.BoolAsInt32(ref B);
            stream.BoolAsInt32(ref A);
            stream.BoolAsInt32(ref Override);
            stream.Guid(ref ExpressionGuid);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FStaticComponentMaskParameter) sourceObj;

            ParameterName = destArchive.MapNameFromSourceArchive(other.ParameterName);
            R = other.R;
            G = other.G;
            B = other.B;
            A = other.A;
            Override = other.Override;
            ExpressionGuid = other.ExpressionGuid;
        }

        public void PopulateDependencies(List<int> dependencyIndices)
        {
        }
    }
}
