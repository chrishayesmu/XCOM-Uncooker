using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCOM_Uncooker.Unreal.Physical
{
    public struct FGenerationInfo
    {
        public int ExportCount;
        public int NameCount;
        public int NetObjectCount;

        public override string ToString()
        {
            return $"FGenerationInfo=(ExportCount={ExportCount}, NameCount={NameCount}, NetObjectCount={NetObjectCount})";
        }
    }

    /// <summary>
    /// Metadata regarding a thumbnail to be shown in the UE3 Content Browser. Not very relevant for
    /// uncooking, but included here for completeness.
    /// </summary>
    public struct FThumbnailMetadata
    {
        public string ClassName;

        public string ObjectPathWithoutPackageName;

        /// <summary>
        /// Position in the package file where the thumbnail data is stored.
        /// </summary>
        public int FileOffset;
    }
}
