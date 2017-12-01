using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze
{
    class Graph
    {
        IDictionary<Package, GraphNode> _nodeMap;

        public Graph(IEnumerable<Package> packages)
        {
            _nodeMap = packages.Where(r => r.IsModulePackage && r.Assembly != null)
                .Select(r => new GraphNode(r)).ToDictionary(r => r.Package);
        }

        public void Build(IDictionary<string, Package> map)
        {
            foreach (var node in _nodeMap.Values)
            {
                if (node.Package.Dependencies == null || node.Package.Dependencies.Count == 0)
                    continue;

                foreach (var dependency in node.Package.Dependencies)
                {
                    var dependencyPackage = map[dependency.Id];

                    if (!_nodeMap.TryGetValue(dependencyPackage, out var dependencyNode))
                        continue;

                    dependencyNode.Dependents.Add(node);
                    node.Dependencies.Add(dependencyNode);
                }
            }
        }

        public string[] GenerateSortedModuleList()
        {
            var queue = new Queue<GraphNode>(_nodeMap.Count);

            foreach (var node in _nodeMap.Values)
                if (node.Dependencies.Count == 0)
                    queue.Enqueue(node);

            var sortedCount = 0;
            var result = new string[_nodeMap.Count];

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                result[sortedCount++] = node.Package.Id;

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
