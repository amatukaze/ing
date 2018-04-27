using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sakuno.ING.Bootstrap;

namespace Sakuno.ING
{
    public static class Program
    {
        private static readonly string[] modules =
        {
            "Sakuno.ING.Core.CommonSettings",
            "Sakuno.ING.Core.Data.Desktop",
            "Sakuno.ING.Core.DateTime",
            "Sakuno.ING.Core.Shell.Desktop",
            "Sakuno.ING.Data",
        };

        [STAThread]
        public static void Main(string[] args)
        {
            var emptyDictionary = new Dictionary<string, string>();
            Bootstraper.Initialize(args, modules
                .Select(Assembly.Load)
                .Select(asm => new PackageStartupInfo
                {
                    Id = asm.GetName().Name,
                    Version = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                                ?? asm.GetName().Version.ToString(),
                    Module = new Lazy<Assembly>(() => asm),
                    Dependencies = emptyDictionary
                }), null);
            Bootstraper.Startup();
        }
    }
}
