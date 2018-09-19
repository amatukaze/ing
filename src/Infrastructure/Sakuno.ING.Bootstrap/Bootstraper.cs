using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Sakuno.ING.Composition;
using Sakuno.ING.Services;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Bootstrap
{
    public static class Bootstraper
    {
        public static bool IsInitialized { get; private set; }

        public static void InitializeFromAssemblyNames(Type visualElementType, string[] commandLine, params string[] assemblyNames)
        {
            var emptyDictionary = new Dictionary<string, string>();

            Initialize(visualElementType, commandLine, assemblyNames
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

        public static void Initialize(Type visualElementType, string[] commandLine, IEnumerable<PackageStartupInfo> packages, IPackageStorage storage)
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

            var builder = new ContainerBuilder();
            var eager = new HashSet<Type>();
            foreach (var a in assemblies)
                foreach (var t in a.DefinedTypes)
                {
                    IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> b = null;
                    foreach (var attr in t.GetCustomAttributes())
                        switch (attr)
                        {
                            case ExportAttribute export:
                                if (b == null)
                                    b = builder.RegisterType(t);
                                b.As(export.ContractType);
                                if (export.SingleInstance)
                                    b.SingleInstance();
                                else
                                    b.InstancePerDependency();
                                if (!export.LazyCreate)
                                    eager.Add(export.ContractType);
                                break;
                            case ExportViewAttribute view:
                                builder.RegisterType(t).Named(view.ViewId, visualElementType).InstancePerDependency();
                                break;
                            case ExportSettingViewAttribute setting:
                                builder.RegisterType(t).WithMetadata(FlexibleShell<object>.SettingCategoryName, setting.Category).As(visualElementType);
                                break;
                        }
                }

            builder.RegisterInstance(new ModuleList(moduleInfos)).As<IModuleList>();
            builder.RegisterInstance(new PackageService(packages, storage)).As<IPackageService>();
            var c = builder.Build();
            var compositor = new AutoFacCompositor(c);
            foreach (var t in eager)
                compositor.Resolve(t.MakeArrayType());
        }

        public static void Startup()
        {
            if (Compositor.Default == null)
                throw new InvalidOperationException("Bootstrapper not initialized.");
            Compositor.Static<IShell>().Run();
        }
    }
}
