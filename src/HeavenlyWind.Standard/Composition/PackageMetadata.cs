using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Composition
{
    public sealed class PackageMetadata
    {
        public string Id { get; }
        public string Version { get; }
        public string Author { get; }
        public string Title { get; }
        public string Description { get; }
        public IReadOnlyDictionary<string, string> Dependencies { get; }
    }
}
