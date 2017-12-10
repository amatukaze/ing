using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using PackageContainer = System.IO.Packaging.Package;

namespace Sakuno.KanColle.Amatsukaze
{
    static partial class Program
    {
        const string PackagesDirectoryName = "Packages";
        const string StagingPackagesDirectoryName = "Staging";

        const string FoundationPackageName = "HeavenlyWind.Foundation";
        const string LauncherPackageName = "HeavenlyWind.Launcher";
        const string BootstrapPackageName = "HeavenlyWind.Bootstrap";

        static string _currentDirectory;
        static string _stagingPackagesDirectory;

        static IDictionary<string, Package> _installedPackages;
        static ISet<PackageInfo> _absentPackages = new HashSet<PackageInfo>();

        static bool needRestart;

        static void Main(string[] args)
        {
            _defaultConsoleColor = Console.ForegroundColor;

            var currentAssembly = Assembly.GetEntryAssembly();
            var oldLauncher = new FileInfo(currentAssembly.Location + ".old");
            if (oldLauncher.Exists)
                oldLauncher.Delete();

            _currentDirectory = Path.GetDirectoryName(currentAssembly.Location);
            Package.Directory = Path.Combine(_currentDirectory, PackagesDirectoryName);
            _stagingPackagesDirectory = Path.Combine(_currentDirectory, StagingPackagesDirectoryName);

            var versionAttribute = currentAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            Print("Launcher version: ");
            PrintLine(versionAttribute.InformationalVersion, ConsoleColor.Yellow);

            PrintLine();

            if (Directory.Exists(_stagingPackagesDirectory))
                ExtractPackages();

            Directory.CreateDirectory(Package.Directory);
            LoadInstalledPackages();

            if (!SelfTest())
            {
                Directory.CreateDirectory(_stagingPackagesDirectory);

                PrintLine();

                while (!(_absentPackages.Count == 0 ?
                    DownloadLastestFoundation() :
                    DownloadDependencies()))
                {
                    PrintLine("Press the keyboard to retry.");

                    Console.ReadKey();

                    PrintLine();
                }

                needRestart = true;
            }

            if (needRestart)
            {
                PrintLine("Restart in 3s...");

                Task.Delay(3000).Wait();

                Process.Start(currentAssembly.Location);

                return;
            }

            PrintLine("Building module graph...");

            var graph = new Graph(_installedPackages.Values);

            graph.Build(_installedPackages);

            var modules = graph.GenerateSortedModuleList();
            if (modules == null)
            {
                PrintLine("Circular dependency detected. Failed to boot.");

                Console.ReadKey();
                return;
            }

            PrintLine();
            PrintLine("Ready to boot");

            StartupNormally(args, modules);
        }

        static void LoadInstalledPackages()
        {
            _installedPackages = new Dictionary<string, Package>(StringComparer.OrdinalIgnoreCase);

            foreach (var packageDirectory in Directory.EnumerateDirectories(Package.Directory))
            {
                var manifestFilename = Path.Combine(packageDirectory, PackageUtil.PackageManifestFilename);
                if (!File.Exists(manifestFilename))
                    continue;

                var manifest = ManifestUtil.Load(manifestFilename);
                if (manifest == null)
                    continue;

                if (!manifest.ValidateSchema())
                    continue;

                var info = new Package(manifest);

                _installedPackages[info.Id] = info;
            }
        }

        static bool SelfTest()
        {
            Print("Testing foundation manifest ");

            if (!_installedPackages.TryGetValue(FoundationPackageName, out var foundationPackage))
            {
                PrintLine("[NotFound]", ConsoleColor.Red);
                return false;
            }

            PrintLine("[OK]", ConsoleColor.Yellow);

            PrintLine("Testing foundation dependencies:");

            foreach (var dependency in EnumerateDependencies(foundationPackage))
            {
                Print(" - ");
                Print(dependency.Id);
                Print(' ');

                if (dependency.Id != LauncherPackageName && dependency.Assembly == null)
                {
                    PrintLine("[CodebaseNotFound]", ConsoleColor.Red);
                    return false;
                }

                PrintLine("[OK]", ConsoleColor.Yellow);
            }
            return true;
        }
        static IEnumerable<Package> EnumerateDependencies(Package package)
        {
            foreach (var dependencyInfo in package.Dependencies)
            {
                if (!_installedPackages.TryGetValue(dependencyInfo.Id, out var dependencyPackage))
                {
                    _absentPackages.Add(new PackageInfo(dependencyInfo.Id, dependencyInfo.Version));
                    continue;
                }

                yield return dependencyPackage;

                foreach (var subDependency in EnumerateDependencies(dependencyPackage))
                    yield return subDependency;
            }
        }

        static void StartupNormally(string[] args, string[] modules)
        {
            const string BootstrapTypeName = "Sakuno.KanColle.Amatsukaze.Bootstrap.Bootstraper";
            const string BootstrapStartupMethodName = "Startup";
            const string ClassLibraryExtensionName = ".dll";

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            var bootstrapFilename = Path.Combine(Package.Directory, BootstrapPackageName, BootstrapPackageName + ClassLibraryExtensionName);
            var bootstrapAssembly = Assembly.LoadFile(bootstrapFilename);
            var bootstrapType = bootstrapAssembly.GetType(BootstrapTypeName);
            var parameterTypes = new[] { typeof(IDictionary<string, object>) };
            var startupMethod = bootstrapType.GetMethod(BootstrapStartupMethodName, parameterTypes);

            var arguments = new SortedList<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["CommandLine"] = args,
                ["PackageDirectory"] = Package.Directory,
                ["StagingPackageDirectory"] = _stagingPackagesDirectory,
                ["ModuleAssemblies"] = _installedPackages.Values.Where(r => r.IsModulePackage && r.Assembly != null)
                    .ToDictionary(r => r.Id, r => r.Assembly, StringComparer.OrdinalIgnoreCase),
                ["Modules"] = modules,
            };

            var @delegate = (Action<IDictionary<string, object>>)Delegate.CreateDelegate(typeof(Action<IDictionary<string, object>>), startupMethod);

            @delegate(arguments);
        }

        static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name.Remove(args.Name.IndexOf(','));

            if (!_installedPackages.TryGetValue(name, out var info))
                return null;

            return info.Assembly;
        }

        static bool DownloadLastestFoundation()
        {
            const string Url = "https://heavenlywind.cc/api/foundation/lastest?launcher=";

            PrintLine("Get foundation package infos...");

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
        static bool DownloadDependencies() => DownloadPackages("Download missing dependencies:", _absentPackages.ToArray());

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
        static string[] SupportedTFM =
        {
            "net461",
            "net46",
            "net452",
            "net451",
            "net45",
            "net40",
            "net35",
            "net20",
            "netstandard2.0",
            "netstandard1.6",
            "netstandard1.5",
            "netstandard1.4",
            "netstandard1.3",
            "netstandard1.2",
            "netstandard1.1",
            "netstandard1.0",
        };
        static PackageExtractionInfo ExtractPackage(FileInfo file)
        {
            const string ManifestRelationshipType = "http://schemas.microsoft.com/packaging/2010/07/manifest";

            try
            {
                using (var stream = file.OpenRead())
                {
                    var archive = new ZipArchive(stream);
                    var package = PackageContainer.Open(stream);
                    var identifier = package.PackageProperties.Identifier;
                    var directory = Path.Combine(Package.Directory, identifier);
                    var relationship = package.GetRelationshipsByType(ManifestRelationshipType).SingleOrDefault();

                    if (relationship == null)
                        throw new Exception("Manifest not found");

                    var relationshipUri = relationship.TargetUri.OriginalString.Substring(1);

                    foreach (var part in package.GetParts())
                    {
                        var uri = part.Uri.OriginalString.Substring(1);

                        if (uri.StartsWith("_rels/", StringComparison.OrdinalIgnoreCase) ||
                            uri.StartsWith("package/", StringComparison.OrdinalIgnoreCase) ||
                            uri.StartsWith("package/", StringComparison.OrdinalIgnoreCase))
                            continue;

                        var filename = PackageUtil.GetFilename(uri, relationshipUri);

                        ExtractPackagePart(new PackagePartInfo(uri, part, archive.GetEntry(uri)), directory, filename);
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

        static void ReplaceMyself(string directory)
        {
            var launcherFilename = Assembly.GetEntryAssembly().Location;
            var toolsDirectory = Path.Combine(directory, "tools");

            File.Move(launcherFilename, launcherFilename + ".old");
            File.Move(Path.Combine(toolsDirectory, "HeavenlyWind.exe"), launcherFilename);

            var configFilename = Path.Combine(toolsDirectory, "HeavenlyWind.exe.config");
            var currentConfigFilename = launcherFilename + ".config";

            if (File.Exists(currentConfigFilename))
                File.Delete(currentConfigFilename);

            if (File.Exists(configFilename))
                File.Move(configFilename, currentConfigFilename);

            Directory.Delete(toolsDirectory);
            needRestart = true;
        }

        static void ExtractPackagePart(PackagePartInfo info, string packageDirectory, string filename)
        {
            var filepath = Path.Combine(packageDirectory, filename);
            var directory = new DirectoryInfo(Path.GetDirectoryName(filepath));
            if (!directory.Exists)
                directory.Create();

            using (var output = File.Create(filepath))
            using (var partStream = info.Part.GetStream())
                partStream.CopyTo(output);

            File.SetLastWriteTime(filepath, info.Entry.LastWriteTime.LocalDateTime);
        }
    }
}
