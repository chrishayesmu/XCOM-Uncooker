using SharpGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnrealArchiveLibrary.Unreal;

namespace UnrealArchiveLibrary.Graph
{
    internal class DependencyTreeNodeComponent : NodeComponent
    {
        public FArchive Archive;

        public override void Copy(NodeComponent nodeComponent)
        {
            (nodeComponent as DependencyTreeNodeComponent).Archive = Archive;
        }
    }
}
