using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Sakuno.KanColle.Amatsukaze
{
    class Package
    {
        public static string Directory { get; set; }

        public string Id { get; }
        public string Version { get; }

        public bool IsModulePackage { get; }

        public IList<PackageInfo> Dependencies { get; }

        Lazy<Assembly> _assembly;
        public Assembly Assembly => _assembly.Value;

        public Package(XDocument manifest)
        {
            Id = manifest.GetId();
            Version = manifest.GetVersion();

            IsModulePackage = manifest.IsModulePackage();

            Dependencies = manifest.EnumerateDependencies().ToArray();

            _assembly = new Lazy<Assembly>(LoadAssembly);
        }

        Assembly LoadAssembly()
        {
            const string ClassLibraryExtensionName = ".dll";

            var filename = Path.Combine(Directory, Id, Id + ClassLibraryExtensionName);
            if (!File.Exists(filename))
                return null;

            return Assembly.LoadFile(filename);
        }

        public override string ToString() => Id;
    }
}
