using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnrealArchiveLibrary.Graph
{
    /// <summary>
    /// Implementation of a directed acyclic graph, mainly for use in dependency resolution between
    /// Unreal archives. Each node must be unique; node values cannot be repeated. Attempts to add an
    /// existing node to the graph are ignored.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DirectedAcyclicGraph<T>
    {
        public int NodeCount => EdgesByNodes.Count;

        private IDictionary<T, ISet<T>> EdgesByNodes = new Dictionary<T, ISet<T>>();

        public void AddEdge(T from, T to)
        {
            AddNode(from);
            AddNode(to);

            EdgesByNodes[from].Add(to);
        }

        public void AddNode(T node)
        {
            if (!EdgesByNodes.ContainsKey(node))
            {
                EdgesByNodes.Add(node, new HashSet<T>());
            }
        }

        public void TraverseBreadthFirst(Action<T> visitor)
        {
            var nodesByDepth = MapNodesByDepth();
            int targetDepth = 0;

            while (true)
            {
                if (!nodesByDepth.TryGetValue(targetDepth, out var nodes) || nodes.Count == 0)
                {
                    break;
                }

                foreach (var node in nodes)
                {
                    visitor(node);
                }

                targetDepth++;
            }
        }

        /// <summary>
        /// A breadth-first traversal, but the visitor is provided every node for the current
        /// depth at the same time.
        /// </summary>
        /// <param name="visitor"></param>
        public void TraverseBreadthFirstMultiple(Action<IEnumerable<T>, int> visitor)
        {
            var nodesByDepth = MapNodesByDepth();
            int targetDepth = 0;

            while (true)
            {
                if (!nodesByDepth.TryGetValue(targetDepth, out var nodes) || nodes.Count == 0)
                {
                    break;
                }

                visitor(nodes, targetDepth);

                targetDepth++;
            }
        }

        private int CalcNodeDepth(T node)
        {
            int depth = 0;

#if DEBUG
            if (!EdgesByNodes.ContainsKey(node))
            {
                throw new Exception("Couldn't find the specified node");
            }
#endif

            foreach (var otherNode in EdgesByNodes[node])
            {
                depth = Math.Max(depth, CalcNodeDepth(otherNode) + 1);
            }

            return depth;
        }

        private IDictionary<int, List<T>> MapNodesByDepth()
        {
            var nodeDict = new Dictionary<int, List<T>>();

            foreach (var node in EdgesByNodes.Keys)
            {
                int depth = CalcNodeDepth(node);

                if (!nodeDict.ContainsKey(depth))
                {
                    nodeDict.Add(depth, new List<T>());
                }

                nodeDict[depth].Add(node);
            }

            return nodeDict;
        }
    }
}
