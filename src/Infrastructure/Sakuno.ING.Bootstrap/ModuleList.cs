using System.Collections.Generic;

namespace Sakuno.ING.Bootstrap
{
    internal class ModuleList : IModuleList
    {
        public IDictionary<string, ModuleInfo> Modules { get; }

        public ModuleList(IDictionary<string, ModuleInfo> modules)
        {
            Modules = modules;
        }
    }
}
