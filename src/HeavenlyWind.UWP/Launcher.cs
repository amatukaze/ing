using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sakuno.KanColle.Amatsukaze.Bootstrap;

namespace Sakuno.KanColle.Amatsukaze.UWP
{
    internal static class Launcher
    {
        static string[] assemblyNames =
        {
            "HeavenlyWind.UWP.Data",
        };
        public static void Launch()
        {
            var emptyDictionary = new Dictionary<string, string>();

            Bootstraper.Startup(Array.Empty<string>(),
                assemblyNames.Select(Assembly.Load).Prepend(Assembly.GetExecutingAssembly())
                    .Select(asm => new PackageStartupInfo
                    {
                        Id = asm.GetName().Name,
                        Version = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
                            ?? asm.GetName().Version.ToString(),
                        Dependencies = emptyDictionary,
                        Module = new Lazy<Assembly>(asm)
                    }).ToArray(),
                null);
        }
    }
}
