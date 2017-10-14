using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HeavenlyWind
{
    static partial class Program
    {
        const string ModulesDirectoryName = "Modules";

        const string BootstrapAssemblyName = "HeavenlyWind.Bootstrap";
        const string BootstrapTypeName = "HeavenlyWind.Bootstrap.Bootstraper";
        const string BootstrapStartupMethodName = "Startup";

        static void Main(string[] args)
        {
            _defaultConsoleColor = Console.ForegroundColor;

            var currentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            var bootstrapFilename = Path.Combine(currentDirectory, ModulesDirectoryName, BootstrapAssemblyName, BootstrapAssemblyName + ".dll");
            var bootstrapAssembly = Assembly.LoadFile(bootstrapFilename);
            var bootstrapType = bootstrapAssembly.GetType(BootstrapTypeName);
            var startupMethod = bootstrapType.GetMethod(BootstrapStartupMethodName, BindingFlags.Public | BindingFlags.Static);

            startupMethod.Invoke(null, null);
        }
    }
}
