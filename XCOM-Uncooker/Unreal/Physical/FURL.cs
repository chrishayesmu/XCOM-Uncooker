using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;

namespace XCOM_Uncooker.Unreal.Physical
{
    public class FURL
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
            stream.String(out Protocol);
            stream.String(out Host);
            stream.String(out Map);
            stream.String(out Portal);
            stream.StringArray(out Op);
            stream.Int32(out Port);
            stream.Int32(out Valid);
        }
    }
}
