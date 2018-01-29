using Autofac;
using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sakuno.KanColle.Amatsukaze.Bootstrap
{
    public static class Bootstraper
    {
        static IDictionary<string, object> _args;
        static IDictionary<string, Assembly> _moduleAssemblies;
        static IDictionary<string, string> _packageVersions;
        static IPackageService _packageService;

        static IList<IModule> _modules;
        static IDictionary<string, ModuleInfo> _moduleInfos;

        static IContainer _container;
        static IResolver _resolver;

        public static void Startup(IDictionary<string, object> args)
        {
            _args = args;

            _moduleAssemblies = (IDictionary<string, Assembly>)args["ModuleAssemblies"];
            _packageVersions = (IDictionary<string, string>)args["PackageVersions"];
            _packageService = (IPackageService)args["PackageService"];

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

            _resolver.Resolve<App>().Run();
        }

        static void ImportModuleTypes()
        {
            _moduleInfos = new Dictionary<string, ModuleInfo>(StringComparer.OrdinalIgnoreCase);

            foreach (var item in _moduleAssemblies)
            {
                var packageId = item.Key;

                var isDuplicate = false;

                foreach (var moduleType in item.Value.GetTypes().Where(r => r.IsConcrete() && r.IsAssignableTo<IModule>()))
                {
                    if (isDuplicate)
                        continue;

                    isDuplicate = true;

                    var info = ModuleInfo.Create(packageId, _packageVersions[packageId], moduleType);

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

            containerBuilder.Register(_ => new Resolver(_container)).SingleInstance().As<IResolver>();

            containerBuilder.RegisterInstance(new ModuleList(_moduleInfos)).SingleInstance().As<IModuleList>();
            containerBuilder.RegisterInstance(_packageService).SingleInstance().As<IPackageService>();

            containerBuilder.RegisterType<App>().SingleInstance();

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
