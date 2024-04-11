using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XCOM_Uncooker.IO;
using XCOM_Uncooker.Unreal.Physical;

namespace XCOM_Uncooker.Unreal
{
    public interface IUnrealSerializable
    {
        /// <summary>
        /// This object's owner, somewhere up the hierarchy. Note that most objects don't care who their owner
        /// is and won't implement this property, leaving it to be null all the time. Only a few special situations
        /// bother to implement this.
        /// </summary>
        public UObject Owner 
        {
            get => null;
            set { }
        }

        public void Serialize(IUnrealDataStream stream);
    }
}
