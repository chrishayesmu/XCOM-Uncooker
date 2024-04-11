using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical.ObjectSubtypes.Materials
{
    public class FStaticSwitchParameter : IUnrealSerializable
    {
        #region Serialized data

        public FName ParameterName;

        public bool Value;

        public bool Override;

        public Guid ExpressionGuid;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.Name(out ParameterName);
            stream.BoolAsInt32(out Value);
            stream.BoolAsInt32(out Override);
            stream.Guid(out ExpressionGuid);
        }
    }
}
