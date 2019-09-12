using Autofac;
using Sakuno.ING.Composition;
using Sakuno.ING.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sakuno.ING
{
    static class Bootstrapper
    {
        [STAThread]
        static void Main(string[] args)
        {
            BuildContainer();
            Startup();
        }

        static void BuildContainer()
        {
            var assemblies = new[]
            {
                "HeavenlyWind.Core",
                "HeavenlyWind.Game",
            }.Select(Assembly.Load).Prepend(Assembly.GetEntryAssembly());

            var builder = new ContainerBuilder();
            var eager = new HashSet<Type>();

            foreach (var assembly in assemblies)
                foreach (var type in assembly.DefinedTypes)
                {
                    var export = type.GetCustomAttribute<ExportAttribute>();
                    if (export == null)
                        continue;

                    var b = builder.RegisterType(type).As(export.ContractType);

                    if (export.SingleInstance)
                        b.SingleInstance();
                    else
                        b.InstancePerDependency();

                    if (!export.LazyCreate)
                        eager.Add(export.ContractType);
                }

            var compositor = new AutofacCompositor(builder.Build());
            foreach (var type in eager)
                compositor.Resolve(type.MakeArrayType());
        }

        static void Startup() => Compositor.Static<IShell>().Run();
    }
}
