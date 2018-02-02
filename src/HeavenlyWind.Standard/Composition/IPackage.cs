using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Composition
{
    public interface IPackage
    {
        string Id { get; }
        string Version { get; }
        bool IsMetaPackage { get; }

        bool IsLoaded { get; }
        bool IsEnabled { get; }

        bool EnabledRequired { get; set; }
        bool RemoveRequired { get; set; }
        IReadOnlyDictionary<string, string> Dependencies { get; }

        ModuleMetadata Metadata { get; }
    }
}
