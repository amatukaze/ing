using System.Collections.Generic;

namespace Sakuno.ING.Bootstrap
{
    class ModuleInfo
    {
        public ModuleInfo(string id, string version, IEnumerable<string> dependsOn)
        {
            Id = id;
            Version = version;
            DependsOn = dependsOn;
        }

        public string Id { get; }

        public string Version { get; }

        public IEnumerable<string> DependsOn { get; }
    }
}
