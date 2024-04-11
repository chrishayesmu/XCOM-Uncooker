using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical
{
    public class FURL : IUnrealSerializable
    {
        #region Serialized data

        public string Protocol;
        public string Host;
        public string Map;
        public string Portal;
        public string[] Op;
        public int Port;
        public int Valid;

        #endregion

        public void Serialize(IUnrealDataStream stream)
        {
            stream.String(ref Protocol);
            stream.String(ref Host);
            stream.String(ref Map);
            stream.String(ref Portal);
            stream.StringArray(ref Op);
            stream.Int32(ref Port);
            stream.Int32(ref Valid);
        }
    }
}
