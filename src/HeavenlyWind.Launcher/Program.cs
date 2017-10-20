using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace HeavenlyWind
{
    static partial class Program
    {
        const string ModulesDirectoryName = "Modules";

        const string FoundationPackageName = "HeavenlyWind.Foundation";
        const string LauncherPackageName = "HeavenlyWind.Launcher";
        const string BootstrapPackageName = "HeavenlyWind.Bootstrap";

        const string BootstrapTypeName = "HeavenlyWind.Bootstrap.Bootstraper";
        const string BootstrapStartupMethodName = "Startup";

        const string ModuleManifestFilename = "module.nuspec";

        const string ClassLibraryExtensionName = ".dll";

        static string _currentDirectory;
        static string _moduleDirectory;

        static void Main(string[] args)
        {
            _defaultConsoleColor = Console.ForegroundColor;

            var currentAssembly = Assembly.GetEntryAssembly();
            _currentDirectory = Path.GetDirectoryName(currentAssembly.Location);
            _moduleDirectory = Path.Combine(_currentDirectory, ModulesDirectoryName);

            var versionAttribute = currentAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            Print("Launcher version: ");
            PrintLine(versionAttribute.InformationalVersion, ConsoleColor.Yellow);

            PrintLine();

            var statusNames = GetStatusNames();

            foreach (var result in EnsureFoundationModules(statusNames))
            {
                Print(' ');

                if (result < StatusCode.Failed)
                {
                    PrintLine(statusNames[(int)result], ConsoleColor.Yellow);
                    continue;
                }

                PrintLine(statusNames[(int)result], ConsoleColor.Red);
                PrintLine();

                return;
            }

            var bootstrapFilename = Path.Combine(_moduleDirectory, BootstrapPackageName, BootstrapPackageName + ClassLibraryExtensionName);
            var bootstrapAssembly = Assembly.LoadFile(bootstrapFilename);
            var bootstrapType = bootstrapAssembly.GetType(BootstrapTypeName);
            var startupMethod = bootstrapType.GetMethod(BootstrapStartupMethodName, BindingFlags.Public | BindingFlags.Static);

            startupMethod.Invoke(null, null);
        }

        static string[] GetStatusNames()
        {
            var values = (StatusCode[])Enum.GetValues(typeof(StatusCode));
            var result = new string[values.Length];

            for (var i = 0; i < values.Length; i++)
                result[i] = "[" + values[i].ToString() + "]";

            return result;
        }

        static IEnumerable<StatusCode> EnsureFoundationModules(string[] statusNames)
        {
            Print("Searching for foundation manifest");

            var foundationManifestFilename = Path.Combine(_moduleDirectory, FoundationPackageName, ModuleManifestFilename);
            if (File.Exists(foundationManifestFilename))
                yield return StatusCode.Found;
            else
                yield return StatusCode.ManifestNotFound;

            Print("Loading foundation manifest");

            var foundationManifest = ManifestUtil.Load(foundationManifestFilename);
            if (foundationManifest != null)
                yield return StatusCode.Success;
            else
                yield return StatusCode.ManifestBadFormat;

            Print("Verifying foundation manifest");

            if (foundationManifest.ValidateSchema())
                yield return StatusCode.Success;
            else
                yield return StatusCode.Failed;

            PrintLine("Checking dependencies:");

            var allSuccess = true;

            foreach (var dependency in EnsureDependencies(foundationManifest))
            {
                Print(" - ");
                Print(dependency.Name);
                Print(' ');

                if (dependency.StatusCode < StatusCode.Failed)
                {
                    PrintLine(statusNames[(int)dependency.StatusCode], ConsoleColor.Yellow);
                    continue;
                }

                PrintLine(statusNames[(int)dependency.StatusCode], ConsoleColor.Red);

                allSuccess = false;
            }

            Print("Ready to boot");

            if (allSuccess)
                yield return StatusCode.Success;
            else
                yield return StatusCode.Failed;
        }
        static IEnumerable<DependencyLoadingInfo> EnsureDependencies(XDocument manifest)
        {
            var dependencies = manifest.EnumerateDependencies();
            if (dependencies == null)
                return null;

            return EnsureDependenciesCore(dependencies);
        }
        static IEnumerable<DependencyLoadingInfo> EnsureDependenciesCore(IEnumerable<string> dependencies)
        {
            foreach (var dependency in dependencies)
            {
                var dependencyManifestFilename = Path.Combine(_moduleDirectory, dependency, ModuleManifestFilename);
                if (!File.Exists(dependencyManifestFilename))
                {
                    yield return new DependencyLoadingInfo(dependency, StatusCode.ManifestNotFound);
                    continue;
                }

                var dependencyManifest = ManifestUtil.Load(dependencyManifestFilename);
                if (dependencyManifest == null)
                {
                    yield return new DependencyLoadingInfo(dependency, StatusCode.ManifestBadFormat);
                    continue;
                }

                if (!dependencyManifest.ValidateSchema())
                {
                    yield return new DependencyLoadingInfo(dependency, StatusCode.ManifestBadFormat);
                    continue;
                }

                if (dependencyManifest.GetId() != dependency)
                {
                    yield return new DependencyLoadingInfo(dependency, StatusCode.ManifestMismatch);
                    continue;
                }

                if (dependency != LauncherPackageName)
                {
                    var dependencyCodebaseFilename = Path.Combine(_moduleDirectory, dependency, dependency + ClassLibraryExtensionName);
                    if (!File.Exists(dependencyCodebaseFilename))
                    {
                        yield return new DependencyLoadingInfo(dependency, StatusCode.CodebaseNotFound);
                        continue;
                    }
                }

                yield return new DependencyLoadingInfo(dependency, StatusCode.Ok);

                var subDependencies = EnsureDependencies(dependencyManifest);
                if (subDependencies != null)
                    foreach (var subDependency in subDependencies)
                        yield return subDependency;
            }
        }
    }
}
