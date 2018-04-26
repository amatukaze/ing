using System.Collections.Generic;

namespace Sakuno.ING
{
    class GraphNode
    {
        public Package Package { get; }

        public ISet<GraphNode> Dependencies { get; } = new HashSet<GraphNode>();
        public ISet<GraphNode> Dependents { get; } = new HashSet<GraphNode>();

        public GraphNode(Package package)
        {
            Package = package;
        }

        public override string ToString() => Package.Id;
    }
}
