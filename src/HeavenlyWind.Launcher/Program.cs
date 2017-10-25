using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HeavenlyWind
{
    static partial class Program
    {
        const string ModulesDirectoryName = "Modules";
        const string StagingModulesDirectoryName = "Staging";

        const string FoundationPackageName = "HeavenlyWind.Foundation";
        const string LauncherPackageName = "HeavenlyWind.Launcher";
        const string BootstrapPackageName = "HeavenlyWind.Bootstrap";

        const string BootstrapTypeName = "HeavenlyWind.Bootstrap.Bootstraper";
        const string BootstrapStartupMethodName = "Startup";

        const string ModuleManifestFilename = "module.nuspec";

        const string ClassLibraryExtensionName = ".dll";

        static string _currentDirectory;
        static string _moduleDirectory;
        static string _stagingModulesDirectory;

        static Action _nextStepOnFailure;

        static void Main(string[] args)
        {
            _defaultConsoleColor = Console.ForegroundColor;

            var currentAssembly = Assembly.GetEntryAssembly();
            _currentDirectory = Path.GetDirectoryName(currentAssembly.Location);
            _moduleDirectory = Path.Combine(_currentDirectory, ModulesDirectoryName);
            _stagingModulesDirectory = Path.Combine(_currentDirectory, StagingModulesDirectoryName);

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

                if (_nextStepOnFailure != null)
                {
                    var stagingDirectory = new DirectoryInfo(_stagingModulesDirectory);
                    if (!stagingDirectory.Exists)
                        stagingDirectory.Create();

                    _nextStepOnFailure();
                }
                return;
            }

            StartupNormally();
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
            _nextStepOnFailure = DownloadLastestFoundation;

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

            _nextStepOnFailure = null;

            PrintLine("Checking dependencies:");

            var allSuccess = true;
            var checkedDependencies = new HashSet<DependencyInfo>(new DependencyInfo.Comparer());
            var missingDependencies = new List<DependencyInfo>();

            foreach (var info in EnsureDependencies(foundationManifest, checkedDependencies))
            {
                Print(" - ");
                Print(info.Dependency.Name);
                Print(' ');

                if (info.StatusCode < StatusCode.Failed)
                {
                    PrintLine(statusNames[(int)info.StatusCode], ConsoleColor.Yellow);
                    continue;
                }

                missingDependencies.Add(info.Dependency);

                PrintLine(statusNames[(int)info.StatusCode], ConsoleColor.Red);

                allSuccess = false;
            }

            Print("Ready to boot");

            if (allSuccess)
                yield return StatusCode.Success;
            else
            {
                _nextStepOnFailure = () => DownloadMissingDependencies(missingDependencies);
                yield return StatusCode.Failed;
            }
        }
        static IEnumerable<DependencyLoadingInfo> EnsureDependencies(XDocument manifest, HashSet<DependencyInfo> checkedDependencies)
        {
            var dependencies = manifest.EnumerateDependencies();
            if (dependencies == null)
                return null;

            return EnsureDependenciesCore(dependencies, checkedDependencies);
        }
        static IEnumerable<DependencyLoadingInfo> EnsureDependenciesCore(IEnumerable<DependencyInfo> dependencies, HashSet<DependencyInfo> checkedDependencies)
        {
            foreach (var dependency in dependencies)
            {
                if (!checkedDependencies.Add(dependency))
                    continue;

                var dependencyManifestFilename = Path.Combine(_moduleDirectory, dependency.Name, ModuleManifestFilename);
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

                if (dependencyManifest.GetId() != dependency.Name)
                {
                    yield return new DependencyLoadingInfo(dependency, StatusCode.ManifestMismatch);
                    continue;
                }

                if (dependency.Name != LauncherPackageName)
                {
                    var dependencyCodebaseFilename = Path.Combine(_moduleDirectory, dependency.Name, dependency.Name + ClassLibraryExtensionName);
                    if (!File.Exists(dependencyCodebaseFilename))
                    {
                        yield return new DependencyLoadingInfo(dependency, StatusCode.CodebaseNotFound);
                        continue;
                    }
                }

                yield return new DependencyLoadingInfo(dependency, StatusCode.Ok);

                var subDependencies = EnsureDependencies(dependencyManifest, checkedDependencies);
                if (subDependencies != null)
                    foreach (var subDependency in subDependencies)
                        yield return subDependency;
            }
        }

        static void StartupNormally()
        {
            var bootstrapFilename = Path.Combine(_moduleDirectory, BootstrapPackageName, BootstrapPackageName + ClassLibraryExtensionName);
            var bootstrapAssembly = Assembly.LoadFile(bootstrapFilename);
            var bootstrapType = bootstrapAssembly.GetType(BootstrapTypeName);
            var startupMethod = bootstrapType.GetMethod(BootstrapStartupMethodName, BindingFlags.Public | BindingFlags.Static);

            startupMethod.Invoke(null, null);
        }

        static void DownloadLastestFoundation()
        {

        }

        static void DownloadMissingDependencies(IList<DependencyInfo> dependencies)
        {
            PrintLine("Download missing dependencies:");

            var tasks = dependencies.Select(DownloadMissingDependency).ToArray();

            try
            {
                Task.WaitAll(tasks);
            }
            catch (AggregateException)
            {
            }

            for (var i = 0; i < tasks.Length; i++)
            {
                Print(" - ");
                Print(dependencies[i].ToString());
                Print(' ');

                if (tasks[i].Status == TaskStatus.RanToCompletion)
                    PrintLine("[Success]", ConsoleColor.Yellow);
                else
                {
                    var exception = tasks[i].Exception.InnerException;

                    PrintLine("[Failed]", ConsoleColor.Red);
                    Print("      ");
                    PrintLine(exception.Message);
                }
            }
        }
        static async Task DownloadMissingDependency(DependencyInfo dependency)
        {
            const string FilenameFormat = "{0}.{1}.nupkg";
            const string Format = "https://api.nuget.org/v3-flatcontainer/{0}/{1}/" + FilenameFormat;

            var request = WebRequest.CreateHttp(string.Format(Format, dependency.Name, dependency.Version));

            using (var md5 = new MD5CryptoServiceProvider())
            using (var response = await request.GetResponseAsync())
            {
                var responseStream = response.GetResponseStream();
                var filename = Path.Combine(_stagingModulesDirectory, string.Format(FilenameFormat, dependency.Name, dependency.Version));
                var file = new FileInfo(filename);
                var tempFilename = filename + ".tmp";

                using (var fileStream = File.Create(tempFilename))
                using (var cryptoStream = new CryptoStream(fileStream, md5, CryptoStreamMode.Write))
                {
                    const int BufferSize = 8192;

                    var buffer = new byte[BufferSize];
                    var count = 0;

                    while ((count = await responseStream.ReadAsync(buffer, 0, BufferSize)) > 0)
                        cryptoStream.Write(buffer, 0, count);
                }

                if (file.Exists)
                    file.Delete();

                File.Move(tempFilename, filename);
            }
        }
    }
}
