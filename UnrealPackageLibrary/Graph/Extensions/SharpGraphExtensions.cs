using SharpGraph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnrealArchiveLibrary.Graph.Extensions
{
    static internal class SharpGraphExtensions
    {
        public static Node GetOrAddNode(this SharpGraph.Graph graph, string label)
        {
            var node = graph.GetNode(label);

            if (node == null)
            {
                node = new Node(label);
                graph.AddNode(node.Value);
            }

            return node.Value;
        }
    }
}
