using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Composition
{
    public sealed class ModuleMetadata
    {
        public ModuleMetadata(string name, string author, string version, string description, IReadOnlyDictionary<string, string> dependsOn)
        {
            Name = name;
            Author = author;
            Version = version;
            Description = description;
            DependsOn = dependsOn;
        }

        public string Name { get; }
        public string Author { get; }
        public string Version { get; }
        public string Description { get; }

        public IReadOnlyDictionary<string, string> DependsOn { get; }
    }
}
