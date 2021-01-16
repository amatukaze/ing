using DryIoc;
using Sakuno.ING.Composition;
using Sakuno.ING.Shell;
using Splat;
using Splat.DryIoc;
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
                                container.Register(exportAttribute.ContractType ?? type, type, exportAttribute.LazyCreate ? null : Reuse.Singleton);
                                break;
                        }
                }

            Locator.CurrentMutable.InitializeSplat();

            new DryIocCompositor(container);
        }

        public static void Startup()
        {
            Compositor.Default.Resolve<IShell>().Run();
        }
    }
}
