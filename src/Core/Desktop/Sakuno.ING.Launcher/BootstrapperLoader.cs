using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Sakuno.ING.Bootstrap;

namespace Sakuno.ING
{
    static class BootstrapperLoader
    {
        public static void Startup(string[] commandLine, IEnumerable<Package> packages)
        {
            Bootstraper.Initialize(typeof(FrameworkElement), commandLine, packages.Select(p => new PackageStartupInfo
            {
                Id = p.Id,
                Version = p.Version,
                Module = p.IsModulePackage ? p.MainAssembly?.LazyAssembly : null,
                Dependencies = p.Dependencies.ToDictionary(d => d.Id, d => d.Version, StringComparer.OrdinalIgnoreCase)
            }), new PackageStorage(ManifestUtil.TargetFrameworks));

            Bootstraper.Startup();
        }
    }
}
