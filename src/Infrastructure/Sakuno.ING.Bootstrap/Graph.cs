using System.Collections.Generic;
using System.Linq;

namespace Sakuno.ING.Bootstrap
{
    internal class Graph
    {
        private readonly IDictionary<ModuleInfo, GraphNode> _nodeMap;

        public Graph(IEnumerable<ModuleInfo> modules)
        {
            _nodeMap = modules.Select(r => new GraphNode(r)).ToDictionary(r => r.Module);
        }

        public void Build(IDictionary<string, ModuleInfo> map)
        {
            foreach (var node in _nodeMap.Values)
            {
                foreach (var dependency in node.Module.DependsOn)
                {
                    var dependencyPackage = map[dependency];

                    if (!_nodeMap.TryGetValue(dependencyPackage, out var dependencyNode))
                        continue;

                    dependencyNode.Dependents.Add(node);
                    node.Dependencies.Add(dependencyNode);
                }
            }
        }

        public ModuleInfo[] GenerateSortedModuleList()
        {
            var queue = new Queue<GraphNode>(_nodeMap.Count);

            foreach (var node in _nodeMap.Values)
                if (node.Dependencies.Count == 0)
                    queue.Enqueue(node);

            var sortedCount = 0;
            var result = new ModuleInfo[_nodeMap.Count];

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                result[sortedCount++] = node.Module;

                foreach (var dependent in node.Dependents)
                {
                    var dependencies = dependent.Dependencies;

                    dependencies.Remove(node);

                    if (dependencies.Count == 0)
                        queue.Enqueue(dependent);
                }
            }

            if (sortedCount < result.Length)
                return null;

            return result;
        }
    }
}
