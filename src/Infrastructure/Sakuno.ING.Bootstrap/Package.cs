using System.Collections.Generic;
using Sakuno.ING.Composition;

namespace Sakuno.ING.Bootstrap
{
    class Package : IPackage
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public bool IsMetaPackage { get; set; }
        public bool IsLoaded { get; set; }
        public bool IsEnabled { get; set; }
        public bool EnabledRequired { get; set; }
        public bool RemoveRequired { get; set; }
        public IReadOnlyDictionary<string, string> Dependencies { get; set; }
        public ModuleMetadata Metadata { get; set; }
    }
}
