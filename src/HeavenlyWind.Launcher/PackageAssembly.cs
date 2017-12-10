using System;
using System.IO;
using System.Reflection;

namespace Sakuno.KanColle.Amatsukaze
{
    class PackageAssembly
    {
        public string AssemblyName { get; }

        public string Package { get; }

        private readonly string filename;
        private readonly Lazy<Assembly> assembly;
        public Assembly Assembly => assembly.Value;

        public PackageAssembly(string assemblyName, string package, string filename)
        {
            AssemblyName = assemblyName;
            Package = package;
            this.filename = filename;
            assembly = new Lazy<Assembly>(LoadAssembly);
        }

        public Assembly LoadAssembly() => File.Exists(filename) ? Assembly.LoadFile(filename) : null;
    }
}
