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
            stream.Name(out ParameterName);
            stream.BoolAsInt32(out R);
            stream.BoolAsInt32(out G);
            stream.BoolAsInt32(out B);
            stream.BoolAsInt32(out A);
            stream.BoolAsInt32(out Override);
            stream.Guid(out ExpressionGuid);
        }
    }
}
