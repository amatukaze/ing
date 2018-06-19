using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sakuno.ING.Composition;
using Sakuno.ING.Services;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Bootstrap
{
    public static class Bootstraper
    {
        public static bool IsInitialized { get; private set; }

        public static void InitializeFromAssemblyNames(string[] commandLine, params string[] assemblyNames)
        {
            var emptyDictionary = new Dictionary<string, string>();

            Initialize(commandLine, assemblyNames
                .Select(Assembly.Load).Prepend(Assembly.GetCallingAssembly())
                .Select(asm => new PackageStartupInfo
                {
                    Id = asm.GetName().Name,
                    Version = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                                ?? asm.GetName().Version.ToString(),
                    Module = new Lazy<Assembly>(() => asm),
                    Dependencies = emptyDictionary
                }), null);
        }

        public static void Initialize(string[] commandLine, IEnumerable<PackageStartupInfo> packages, IPackageStorage storage)
        {
            if (IsInitialized)
                throw new InvalidOperationException("Bootstrapper can only be initialized once.");
            IsInitialized = true;

            var currentAssembly = typeof(Bootstraper).Assembly;
            var versionAttribute = currentAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            Console.WriteLine(@"
       || ||
     __||_||__    ( I'm running! )
    ||.\_|_/.||  O
    =|@ ___ @|= o
    ||_/|||\_||
      |-----|/
     /|     | ");

            Console.WriteLine();
            Console.Write("Bootstrap version: ");
            var foregroundColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(versionAttribute.InformationalVersion);
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine();

            var moduleInfos = packages.ToDictionary(p => p.Id, p => new ModuleInfo(p.Id, p.Version, p.Dependencies.Keys), StringComparer.OrdinalIgnoreCase);
            var assemblies = packages.Select(p => p.Module.Value).ToArray();
            var compositor = new Compositor(assemblies
                .SelectMany(a => a.DefinedTypes
                .SelectMany(t => t.GetCustomAttributes<ExportAttribute>()
                .Select(attr => new Export
                {
                    Implementation = t,
                    Contract = attr.ContractType,
                    SingleInstance = attr.SingleInstance,
                    LazyCreate = attr.LazyCreate
                }))));
            compositor.AttachInstance<IModuleList>(new ModuleList(moduleInfos));
            compositor.AttachInstance<IPackageService>(new PackageService(packages, storage));
            Compositor.SetDefault(compositor);
        }

        public static void Startup()
        {
            if (Compositor.Default == null)
                throw new InvalidOperationException("Bootstrapper not initialized.");
            Compositor.Default.Resolve<IShell>().Run();
        }
    }
}
