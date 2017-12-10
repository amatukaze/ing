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
        public static string BaseDirectory { get; set; }

        public string Id { get; }
        public string Version { get; }

        public bool IsModulePackage { get; }

        public IEnumerable<PackageInfo> Dependencies { get; }

        public string SelectedTFM { get; }

        private readonly string selectedTFMFolder;

        Lazy<Assembly> _assembly;
        public Assembly Assembly => _assembly.Value;

        static string[] supportedTFMs =
        {
            "net461",
            "net46",
            "net452",
            "net451",
            "net45",
            "net40",
            "net35",
            "net20",
            "netstandard2.0",
            "netstandard1.6",
            "netstandard1.5",
            "netstandard1.4",
            "netstandard1.3",
            "netstandard1.2",
            "netstandard1.1",
            "netstandard1.0",
        };

        public Package(XDocument manifest)
        {
            Id = manifest.GetId();
            Version = manifest.GetVersion();

            IsModulePackage = manifest.IsModulePackage();

            Dependencies = manifest.EnumerateDependencies().ToArray();

            var baseFolder = Path.Combine(BaseDirectory, Id, "lib");
            foreach (var tfm in supportedTFMs)
            {
                var folder = Path.Combine(baseFolder, tfm);
                if (Directory.Exists(folder))
                {
                    SelectedTFM = tfm;
                    selectedTFMFolder = folder;
                    break;
                }
            }

            _assembly = new Lazy<Assembly>(LoadAssembly);
        }

        Assembly LoadAssembly()
        {
            const string ClassLibraryExtensionName = ".dll";

            var filename = Path.Combine(BaseDirectory, Id, Id + ClassLibraryExtensionName);
            if (!File.Exists(filename))
                return null;

            return Assembly.LoadFile(filename);
        }

        public override string ToString() => Id;
    }
}
