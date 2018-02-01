using System;
using System.Collections.Generic;
using System.Reflection;

namespace Sakuno.KanColle.Amatsukaze.Bootstrap
{
    public sealed class PackageStartupInfo
    {
        public string Id;

        public string Version;

        public Lazy<Assembly> Module;

        public IReadOnlyDictionary<string, string> Dependencies;
    }
}
