using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Services;
using Sakuno.KanColle.Amatsukaze.Shell;

namespace Sakuno.KanColle.Amatsukaze.Bootstrap
{
    public static class Bootstraper
    {
        static IEnumerable<PackageStartupInfo> _packages;
        static IPackageStorage _storage;

        static IList<IModule> _modules;
        static IDictionary<string, ModuleInfo> _moduleInfos;

        static IContainer _container;
        static IResolver _resolver;

        public static void Startup(string[] commandLine, IEnumerable<PackageStartupInfo> packages, IPackageStorage storage)
        {
            _packages = packages;
            _storage = storage;

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

            ImportModuleTypes();
            ComposeModules();
            InitializeModules();

            _resolver.Resolve<IShell>().Run();
        }

        static void ImportModuleTypes()
        {
            _moduleInfos = new Dictionary<string, ModuleInfo>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in _packages)
            {
                if (item.Module == null) continue;
                var packageId = item.Id;

                var isDuplicate = false;

                foreach (var moduleType in item.Module.Value.GetTypes().Where(r => r.IsConcrete() && r.IsAssignableTo<IModule>()))
                {
                    if (isDuplicate)
                        continue;

                    isDuplicate = true;

                    var info = new ModuleInfo(packageId, item.Version, item.Dependencies.Keys, moduleType);

                    _moduleInfos.Add(packageId, info);
                }
            }

            var graph = new Graph(_moduleInfos.Values);

            graph.Build(_moduleInfos);

            var orderedModules = graph.GenerateSortedModuleList();

            _modules = orderedModules.Select(r => (IModule)Activator.CreateInstance(r.EntryType)).ToArray();
        }
        static void ComposeModules()
        {
            var containerBuilder = new ContainerBuilder();

            var builder = new Builder(containerBuilder);

            foreach (var exposableModule in _modules.OfType<IExposableModule>())
                exposableModule.Expose(builder);

            builder.Complete();

            containerBuilder.Register(_ => new Resolver(_container)).As<IResolver>().SingleInstance();

            containerBuilder.RegisterInstance(new ModuleList(_moduleInfos)).As<IModuleList>().SingleInstance();
            containerBuilder.RegisterInstance(new PackageService(_packages, _storage)).As<IPackageService>().SingleInstance();

            _container = containerBuilder.Build();
        }

        static void InitializeModules()
        {
            _resolver = _container.Resolve<IResolver>();

            foreach (var module in _modules)
                module.Initialize(_resolver);
        }
    }
}
