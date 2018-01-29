using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Composition
{
    public interface IModuleMetadata
    {
        string Name { get; }
        string Author { get; }
        string Version { get; }
        string Description { get; }

        int TargetStandardVersion { get; }

        IEnumerable<string> DependsOn { get; }
    }
}
