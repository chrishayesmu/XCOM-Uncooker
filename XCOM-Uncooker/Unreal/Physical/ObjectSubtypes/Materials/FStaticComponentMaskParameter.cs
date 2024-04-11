using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Materials
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
    }
}
