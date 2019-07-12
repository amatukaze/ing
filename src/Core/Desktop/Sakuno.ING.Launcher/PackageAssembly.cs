using System;
using System.IO;
using System.Reflection;

namespace Sakuno.ING
{
    internal class PackageAssembly
    {
        public string AssemblyName { get; }

        public string Package { get; }

        private readonly string filename;
        public Lazy<Assembly> LazyAssembly { get; }

        public PackageAssembly(string assemblyName, string package, string filename)
        {
            AssemblyName = assemblyName;
            Package = package;
            this.filename = filename;
            LazyAssembly = new Lazy<Assembly>(LoadAssembly);
        }

        public Assembly LoadAssembly() => File.Exists(filename) ? Assembly.LoadFile(filename) : null;
    }
}
