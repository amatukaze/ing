using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.Packaging;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sakuno.KanColle.Amatsukaze
{
    static partial class Program
    {
        const string PackagesDirectoryName = "Packages";
        const string StagingPackagesDirectoryName = "Staging";

        const string FoundationPackageName = "HeavenlyWind.Foundation";
        const string LauncherPackageName = "HeavenlyWind.Launcher";
        const string BootstrapPackageName = "HeavenlyWind.Bootstrap";

        const string ClassLibraryExtensionName = ".dll";

        static string _currentDirectory;
        static string _packageDirectory;
        static string _stagingPackagesDirectory;

        static string[] _statusNames;

        static Func<bool> _nextStepOnFailure;

        static SortedList<string, Assembly> _dependencyAssemblies;
        static string[] _packagesUsedByFoundation;

        static void Main(string[] args)
        {
            _defaultConsoleColor = Console.ForegroundColor;

            var currentAssembly = Assembly.GetEntryAssembly();
            var oldLauncher = new FileInfo(currentAssembly.Location + ".old");
            if (oldLauncher.Exists)
                oldLauncher.Delete();

            _currentDirectory = Path.GetDirectoryName(currentAssembly.Location);
            _packageDirectory = Path.Combine(_currentDirectory, PackagesDirectoryName);
            _stagingPackagesDirectory = Path.Combine(_currentDirectory, StagingPackagesDirectoryName);

            var versionAttribute = currentAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            Print("Launcher version: ");
            PrintLine(versionAttribute.InformationalVersion, ConsoleColor.Yellow);

            PrintLine();

            _statusNames = GetStatusNames();

            if (Directory.Exists(_stagingPackagesDirectory))
                ExtractPackages();

            foreach (var result in EnsureFoundationModules())
            {
                Print(' ');

                if (result < StatusCode.Failed)
                {
                    PrintLine(_statusNames[(int)result], ConsoleColor.Yellow);
                    continue;
                }

                PrintLine(_statusNames[(int)result], ConsoleColor.Red);
                PrintLine();

                if (_nextStepOnFailure != null)
                {
                    var stagingDirectory = new DirectoryInfo(_stagingPackagesDirectory);
                    if (!stagingDirectory.Exists)
                        stagingDirectory.Create();

                    var success = false;

                    do
                    {
                        success = _nextStepOnFailure();

                        PrintLine();

                        if (!success)
                        {
                            PrintLine("Press the keyboard to retry.");

                            Console.ReadKey();

                            PrintLine();
                        }
                    } while (!success);

                    PrintLine("Restart in 3s...");

                    Task.Delay(3000).Wait();

                    Process.Start(currentAssembly.Location);
                }

                return;
            }

            StartupNormally(args);
        }

        static string[] GetStatusNames()
        {
            var values = (StatusCode[])Enum.GetValues(typeof(StatusCode));
            var result = new string[values.Length];

            for (var i = 0; i < values.Length; i++)
                result[i] = "[" + values[i].ToString() + "]";

            return result;
        }

        static IEnumerable<StatusCode> EnsureFoundationModules()
        {
            _nextStepOnFailure = DownloadLastestFoundation;

            Print("Searching for foundation manifest");

            var foundationManifestFilename = Path.Combine(_packageDirectory, FoundationPackageName, PackageUtil.PackageManifestFilename);
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
            var checkedDependencies = new HashSet<PackageInfo>(new PackageInfo.Comparer());
            var missingDependencies = new List<PackageInfo>();

            _dependencyAssemblies = new SortedList<string, Assembly>(StringComparer.OrdinalIgnoreCase);

            foreach (var info in EnsureDependencies(foundationManifest, checkedDependencies))
            {
                Print(" - ");
                Print(info.Dependency.Id);
                Print(' ');

                if (info.StatusCode < StatusCode.Failed)
                {
                    PrintLine(_statusNames[(int)info.StatusCode], ConsoleColor.Yellow);
                    continue;
                }

                missingDependencies.Add(info.Dependency);

                PrintLine(_statusNames[(int)info.StatusCode], ConsoleColor.Red);

                allSuccess = false;
            }

            _packagesUsedByFoundation = _dependencyAssemblies.Keys.ToArray();

            Print("Ready to boot");

            if (allSuccess)
                yield return StatusCode.Success;
            else
            {
                _nextStepOnFailure = () => DownloadPackages("Download missing dependencies:", missingDependencies);
                yield return StatusCode.Failed;
            }
        }
        static IEnumerable<DependencyLoadingInfo> EnsureDependencies(XDocument manifest, HashSet<PackageInfo> checkedDependencies)
        {
            var dependencies = manifest.EnumerateDependencies();
            if (dependencies == null)
                return null;

            return EnsureDependenciesCore(dependencies, checkedDependencies);
        }
        static IEnumerable<DependencyLoadingInfo> EnsureDependenciesCore(IEnumerable<PackageInfo> dependencies, HashSet<PackageInfo> checkedDependencies)
        {
            foreach (var dependency in dependencies)
            {
                if (!checkedDependencies.Add(dependency))
                    continue;

                var dependencyManifestFilename = Path.Combine(_packageDirectory, dependency.Id, PackageUtil.PackageManifestFilename);
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

                if (dependencyManifest.GetId() != dependency.Id)
                {
                    yield return new DependencyLoadingInfo(dependency, StatusCode.ManifestMismatch);
                    continue;
                }

                if (dependency.Id != LauncherPackageName)
                {
                    var dependencyCodebaseFilename = Path.Combine(_packageDirectory, dependency.Id, dependency.Id + ClassLibraryExtensionName);
                    if (!File.Exists(dependencyCodebaseFilename))
                    {
                        yield return new DependencyLoadingInfo(dependency, StatusCode.CodebaseNotFound);
                        continue;
                    }

                    _dependencyAssemblies.Add(dependency.Id, Assembly.LoadFile(dependencyCodebaseFilename));
                }

                yield return new DependencyLoadingInfo(dependency, StatusCode.Ok);

                var subDependencies = EnsureDependencies(dependencyManifest, checkedDependencies);
                if (subDependencies != null)
                    foreach (var subDependency in subDependencies)
                        yield return subDependency;
            }
        }

        static void StartupNormally(string[] args)
        {
            const string BootstrapTypeName = "Sakuno.KanColle.Amatsukaze.Bootstrap.Bootstraper";
            const string BootstrapStartupMethodName = "Startup";

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            var bootstrapFilename = Path.Combine(_packageDirectory, BootstrapPackageName, BootstrapPackageName + ClassLibraryExtensionName);
            var bootstrapAssembly = Assembly.LoadFile(bootstrapFilename);
            var bootstrapType = bootstrapAssembly.GetType(BootstrapTypeName);
            var parameterTypes = new[] { typeof(IDictionary<string, object>) };
            var startupMethod = bootstrapType.GetMethod(BootstrapStartupMethodName, parameterTypes);

            var arguments = new SortedList<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["CommandLine"] = args,
                ["PackageDirectory"] = _packageDirectory,
                ["StagingPackageDirectory"] = _stagingPackagesDirectory,
                ["PackagesUsedByFoundation"] = _packagesUsedByFoundation,
                ["DownloadPackageFunc"] = new Func<string, string, Task>(DownloadPackage),
                ["DependencyAssemblies"] = _dependencyAssemblies,
            };

            ManifestUtil.AddToArguments(arguments);

            var @delegate = (Action<IDictionary<string, object>>)Delegate.CreateDelegate(typeof(Action<IDictionary<string, object>>), startupMethod);

            @delegate(arguments);
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name.Remove(args.Name.IndexOf(','));

            _dependencyAssemblies.TryGetValue(name, out var result);

            return result;
        }

        static bool DownloadLastestFoundation()
        {
            const string Url = "https://heavenlywind.cc/api/foundation/lastest?launcher=";

            PrintLine("Get foundation package infos:");

            var currentAssembly = Assembly.GetEntryAssembly();
            var versionAttribute = currentAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            var request = WebRequest.CreateHttp(Url + versionAttribute.InformationalVersion);
            var packages = new List<PackageInfo>();

            using (var response = request.GetResponse())
            {
                var responseStream = response.GetResponseStream();
                var reader = new StreamReader(responseStream);

                while (!reader.EndOfStream)
                {
                    var id = reader.ReadLine();
                    var version = reader.ReadLine();

                    packages.Add(new PackageInfo(id, version));
                }
            }

            return DownloadPackages("Download foundation:", packages);
        }

        static bool DownloadPackages(string task, IList<PackageInfo> packages)
        {
            PrintLine(task);

            var tasks = packages.Select(r => DownloadPackage(r.Id, r.Version)).ToArray();

            var result = true;

            try
            {
                Task.WaitAll(tasks);
            }
            catch (AggregateException)
            {
                result = false;
            }

            for (var i = 0; i < tasks.Length; i++)
            {
                Print(" - ");
                Print(packages[i].ToString());
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

            return result;
        }
        static async Task DownloadPackage(string id, string version)
        {
            const string FilenameFormat = "{0}.{1}.nupkg";
            const string Format = "https://api.nuget.org/v3-flatcontainer/{0}/{1}/" + FilenameFormat;

            var request = WebRequest.CreateHttp(string.Format(Format, id, version));

            using (var md5 = new MD5CryptoServiceProvider())
            using (var response = await request.GetResponseAsync())
            {
                var responseStream = response.GetResponseStream();
                var filename = Path.Combine(_stagingPackagesDirectory, string.Format(FilenameFormat, id, version));
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

        static void ExtractPackages()
        {
            var stagingPackagesDirectory = new DirectoryInfo(_stagingPackagesDirectory);
            var files = stagingPackagesDirectory.EnumerateFiles("*.nupkg");
            if (files.Any())
            {
                PrintLine("Extracting staging packages:");

                var lockingFilename = Path.Combine(_stagingPackagesDirectory, ".lock");

                using (File.Open(lockingFilename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                    foreach (var file in files)
                    {
                        var info = ExtractPackage(file);

                        Print(" - ");
                        Print(info.Filename);
                        Print(' ');

                        if (info.Exception == null)
                            PrintLine("[Success]", ConsoleColor.Yellow);
                        else
                        {
                            PrintLine("[Failed]", ConsoleColor.Red);
                            Print("      ");
                            PrintLine(info.Exception.Message);
                        }
                    }

                File.Delete(lockingFilename);

                PrintLine();
            }

            Directory.Delete(_stagingPackagesDirectory, true);
        }
        static PackageExtractionInfo ExtractPackage(FileInfo file)
        {
            const string ManifestRelationshipType = "http://schemas.microsoft.com/packaging/2010/07/manifest";
            const int SupportedTargetFrameworkCount = 9;

            try
            {
                using (var stream = file.OpenRead())
                {
                    var archive = new ZipArchive(stream);
                    var package = Package.Open(stream);
                    var identifier = package.PackageProperties.Identifier;
                    var directory = Path.Combine(_packageDirectory, identifier);
                    var relationship = package.GetRelationshipsByType(ManifestRelationshipType).SingleOrDefault();

                    if (relationship == null)
                        throw new Exception("Manifest not found");

                    var relationshipUri = relationship.TargetUri.OriginalString.Substring(1);

                    var libPartInfos = new List<PackagePartInfo>[SupportedTargetFrameworkCount];

                    foreach (var part in package.GetParts())
                    {
                        var uri = part.Uri.OriginalString.Substring(1);

                        if (uri.StartsWith("_rels/", StringComparison.OrdinalIgnoreCase))
                            continue;
                        else if (uri.StartsWith("package/", StringComparison.OrdinalIgnoreCase))
                            continue;

                        if (uri.StartsWith("lib", StringComparison.OrdinalIgnoreCase))
                        {
                            var libTargetFrameworkIndex = -1;

                            if (IsPrefix(uri, "net46"))
                                libTargetFrameworkIndex = 0;
                            else if (IsPrefix(uri, "net452"))
                                libTargetFrameworkIndex = 1;
                            else if (IsPrefix(uri, "net451"))
                                libTargetFrameworkIndex = 2;
                            else if (IsPrefix(uri, "net45"))
                                libTargetFrameworkIndex = 3;
                            else if (IsPrefix(uri, "net40"))
                                libTargetFrameworkIndex = 4;
                            else if (IsPrefix(uri, "netstandard1.3"))
                                libTargetFrameworkIndex = 5;
                            else if (IsPrefix(uri, "netstandard1.2"))
                                libTargetFrameworkIndex = 6;
                            else if (IsPrefix(uri, "netstandard1.1"))
                                libTargetFrameworkIndex = 7;
                            else if (IsPrefix(uri, "netstandard1.0"))
                                libTargetFrameworkIndex = 8;
                            else
                                continue;

                            var parts = libPartInfos[libTargetFrameworkIndex];
                            if (parts == null)
                                libPartInfos[libTargetFrameworkIndex] = parts = new List<PackagePartInfo>();

                            parts.Add(new PackagePartInfo(uri, part, archive.GetEntry(uri)));
                            continue;
                        }

                        var filename = PackageUtil.GetFilenameExceptLibDirectory(uri, relationshipUri);

                        ExtractPackagePart(new PackagePartInfo(uri, part, archive.GetEntry(uri)), directory, filename);
                    }

                    foreach (var partInfos in libPartInfos)
                        if (partInfos != null)
                        {
                            foreach (var info in partInfos)
                            {
                                var filename = PackageUtil.GetFilenameInLibDirectory(info.Name);

                                ExtractPackagePart(info, directory, filename);
                            }

                            break;
                        }

                    if (identifier == LauncherPackageName)
                        ReplaceMyself(directory);
                }

                return new PackageExtractionInfo(file.Name);
            }
            catch (Exception e)
            {
                return new PackageExtractionInfo(file.Name, e);
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool IsPrefix(string uri, string prefix)
        {
            const int LibPrefixLength = 4;

            return uri.IndexOf(prefix, LibPrefixLength, StringComparison.OrdinalIgnoreCase) == LibPrefixLength;
        }

        static void ReplaceMyself(string directory)
        {
            var launcherFilename = Assembly.GetEntryAssembly().Location;
            var toolsDirectory = Path.Combine(directory, "tools");

            File.Move(launcherFilename, launcherFilename + ".old");
            File.Move(Path.Combine(toolsDirectory, "HeavenlyWind.exe"), launcherFilename);

            var configFilename = Path.Combine(toolsDirectory, "HeavenlyWind.exe.config");
            var currentConfigFilename = launcherFilename + ".config";
            var currentConfigFile = new FileInfo(currentConfigFilename);
            if (currentConfigFile.Exists)
                currentConfigFile.Delete();

            File.Move(configFilename, currentConfigFilename);

            Directory.Delete(toolsDirectory);
        }

        static void ExtractPackagePart(PackagePartInfo info, string packageDirectory, string filename)
        {
            var filepath = Path.Combine(packageDirectory, filename);
            var directory = new DirectoryInfo( Path.GetDirectoryName(filepath));
            if (!directory.Exists)
                directory.Create();

            using (var output = File.Create(filepath))
            using (var partStream = info.Part.GetStream())
                partStream.CopyTo(output);

            File.SetLastWriteTime(filepath, info.Entry.LastWriteTime.LocalDateTime);
        }
    }
}
