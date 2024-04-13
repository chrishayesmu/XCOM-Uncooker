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
            stream.Name(ref ParameterName);
            stream.BoolAsInt32(ref Value);
            stream.BoolAsInt32(ref Override);
            stream.Guid(ref ExpressionGuid);
        }

        public void CloneFromOtherArchive(IUnrealSerializable sourceObj, FArchive sourceArchive, FArchive destArchive)
        {
            var other = (FStaticSwitchParameter) sourceObj;

            ParameterName = destArchive.MapNameFromSourceArchive(other.ParameterName);
            Value = other.Value;
            Override = other.Override;
            ExpressionGuid = other.ExpressionGuid;
        }
    }
}
