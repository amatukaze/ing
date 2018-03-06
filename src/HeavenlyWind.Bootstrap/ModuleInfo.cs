using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Bootstrap
{
    class ModuleInfo
    {
        public ModuleInfo(string id, string version, IEnumerable<string> dependsOn, Type entryType)
        {
            Id = id;
            Version = version;
            DependsOn = dependsOn;
            EntryType = entryType;
        }

        public string Id { get; }

        public string Version { get; }

        public IEnumerable<string> DependsOn { get; }

        public Type EntryType { get; }
    }
}
