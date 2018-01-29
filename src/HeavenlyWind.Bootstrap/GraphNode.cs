using System;
using System.Collections.Generic;
using System.Text;

namespace Sakuno.KanColle.Amatsukaze.Bootstrap
{
    class GraphNode
    {
        public ModuleInfo Module { get; }

        public ISet<GraphNode> Dependencies { get; } = new HashSet<GraphNode>();
        public ISet<GraphNode> Dependents { get; } = new HashSet<GraphNode>();

        public GraphNode(ModuleInfo module)
        {
            Module = module;
        }

        public override string ToString() => Module.Id;
    }
}
