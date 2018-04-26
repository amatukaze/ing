using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Sakuno.ING
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

        public PackageAssembly MainAssembly { get; }

        private List<PackageAssembly> _assemblies = new List<PackageAssembly>();
        public IEnumerable<PackageAssembly> Assemblies => _assemblies;

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

            if (selectedTFMFolder != null)
                foreach (var file in Directory.EnumerateFiles(selectedTFMFolder))
                {
                    try
                    {
                        var name = AssemblyName.GetAssemblyName(file);
                        var packageAssembly = new PackageAssembly(name.Name, Id, file);
                        _assemblies.Add(packageAssembly);

                        if (string.Equals(name.Name, Id, StringComparison.OrdinalIgnoreCase))
                            MainAssembly = packageAssembly;
                    }
                    catch { }
                }
        }

        public override string ToString() => Id;
    }
}
