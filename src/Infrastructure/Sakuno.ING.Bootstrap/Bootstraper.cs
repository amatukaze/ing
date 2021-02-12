using DryIoc;
using ReactiveUI;
using Sakuno.ING.Composition;
using Sakuno.ING.Shell;
using Splat;
using Splat.DryIoc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sakuno.ING.Bootstrap
{
    public static class Bootstraper
    {
        public static void Initialize(IEnumerable<string> assemblyNames)
        {
            Initialize(assemblyNames.Select(Assembly.Load));
        }
        public static void Initialize(IEnumerable<Assembly> assemblies)
        {
            var container = new Container();
            var registeredViews = new SortedDictionary<string, Type>(StringComparer.Ordinal);

            container.UseDryIocDependencyResolver();

            foreach (var assembly in assemblies)
                foreach (var type in assembly.DefinedTypes)
                {
                    if (type.IsAbstract)
                        continue;

                    foreach (var attribute in type.GetCustomAttributes())
                        switch (attribute)
                        {
                            case ExportAttribute exportAttribute:
                                container.Register(exportAttribute.ContractType ?? type, type, !exportAttribute.SingleInstance ? null : Reuse.Singleton);
                                break;

                            case ExportViewAttribute exportViewAttribute:
                                registeredViews.Add(exportViewAttribute.ViewId, type);
                                container.Register(type);
                                break;
                        }

                    if (type.IsAssignableTo(typeof(IViewFor)) && !container.IsRegistered(type) &&
                        type.ImplementedInterfaces.SingleOrDefault(r => r.IsGenericType && r.GetGenericTypeDefinition() == typeof(IViewFor<>)) is Type viewForType)
                    {
                        var viewContract = type.GetCustomAttribute<ViewContractAttribute>()?.Contract;

                        container.Register(viewForType, type, serviceKey: viewContract, ifAlreadyRegistered: IfAlreadyRegistered.Throw);
                    }
                }

            Locator.CurrentMutable.InitializeSplat();

            new DryIocCompositor(container, registeredViews);
        }

        public static void Startup()
        {
            Compositor.Default.Resolve<IShell>().Run();
        }
    }
}
