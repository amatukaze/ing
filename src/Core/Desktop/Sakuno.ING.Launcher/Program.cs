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

namespace Sakuno.ING
{
    internal static partial class Program
    {
        private const string PackagesDirectoryName = "Packages";
        private const string StagingPackagesDirectoryName = "Staging";
        private const string FoundationPackageName = "Sakuno.ING.Foundation";
        private const string LauncherPackageName = "Sakuno.ING.Launcher";
        private static string _currentDirectory;
        public static string StagingPackagesDirectory;
        private static IDictionary<string, Package> _installedPackages;
        private static readonly ISet<PackageInfo> _absentPackages = new HashSet<PackageInfo>();
        private static readonly IDictionary<string, PackageAssembly> _installedAssemblies = new Dictionary<string, PackageAssembly>(StringComparer.OrdinalIgnoreCase);
        private static bool needRestart;
        private static bool localDebug;

        [STAThread]
        private static void Main(string[] args)
        {
            _defaultConsoleColor = Console.ForegroundColor;

            if (args.Length > 0 && args[0].Equals("--localdebug", StringComparison.OrdinalIgnoreCase))
            {
                PrintLine("Running in local debug mode", ConsoleColor.Magenta);
                localDebug = true;
            }

            var currentAssembly = Assembly.GetEntryAssembly();
            var oldLauncher = new FileInfo(currentAssembly.Location + ".old");
            if (oldLauncher.Exists)
                oldLauncher.Delete();

            _currentDirectory = Path.GetDirectoryName(currentAssembly.Location);
            Package.BaseDirectory = Path.Combine(_currentDirectory, PackagesDirectoryName);
            StagingPackagesDirectory = Path.Combine(_currentDirectory, StagingPackagesDirectoryName);

            var versionAttribute = currentAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            Print("Launcher version: ");
            PrintLine(versionAttribute.InformationalVersion, ConsoleColor.Yellow);

            PrintLine();

            if (Directory.Exists(StagingPackagesDirectory))
                ExtractPackages();

            Directory.CreateDirectory(Package.BaseDirectory);
            LoadInstalledPackages();

            if (!SelfTest())
            {
                Directory.CreateDirectory(StagingPackagesDirectory);

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

                Process.Start(currentAssembly.Location, localDebug ? "--localdebug" : string.Empty);

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

            foreach (var package in _installedPackages.Values)
            {
                if (package.Id == LauncherPackageName) continue;
                foreach (var assembly in package.Assemblies)
                {
                    if (_installedAssemblies.TryGetValue(assembly.AssemblyName, out var installed))
                    {
                        Print("Dependency conflict: ");
                        PrintLine(assembly.AssemblyName);
                        Print(assembly.Package);
                        Print(" | ");
                        PrintLine(installed.Package);

                        Console.ReadKey();
                        return;
                    }
                    _installedAssemblies.Add(assembly.AssemblyName, assembly);
                }
            }

            PrintLine();
            PrintLine("Ready to boot");

            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            StartupNormally(args);
        }

        private static void LoadInstalledPackages()
        {
            _installedPackages = new Dictionary<string, Package>(StringComparer.OrdinalIgnoreCase);

            foreach (var packageDirectory in Directory.EnumerateDirectories(Package.BaseDirectory))
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

        private static bool SelfTest()
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

                if (dependency.Id != LauncherPackageName && dependency.SelectedTFM == null)
                {
                    PrintLine("[CodebaseNotFound]", ConsoleColor.Red);
                    return false;
                }

                PrintLine("[OK]", ConsoleColor.Yellow);
            }
            return true;
        }

        private static IEnumerable<Package> EnumerateDependencies(Package package)
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

        private static void StartupNormally(string[] args) => BootstrapperLoader.Startup(args, _installedPackages.Values);

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = args.Name.Remove(args.Name.IndexOf(','));

            if (!_installedAssemblies.TryGetValue(name, out var info))
                return null;

            return info.LazyAssembly.Value;
        }

        private static bool DownloadLastestFoundation()
        {
            var packages = new List<PackageInfo>();

            if (localDebug)
            {
                packages.Add(new PackageInfo("Sakuno.ING.Foundation", "0.1.0-blueprint2"));
                packages.Add(new PackageInfo("Sakuno.ING.Launcher", "0.1.0-blueprint2"));
                packages.Add(new PackageInfo("Sakuno.ING.Bootstrap", "0.1.0-blueprint2"));
                packages.Add(new PackageInfo("Sakuno.ING.Standard", "0.1.0-blueprint"));
                packages.Add(new PackageInfo("Sakuno.Base", "0.3.1"));
                packages.Add(new PackageInfo("Newtonsoft.Json", "11.0.1"));
                packages.Add(new PackageInfo("Autofac", "4.6.2"));
            }
            else
            {
                const string Url = "https://heavenlywind.cc/api/foundation/lastest?launcher=";

                PrintLine("Get foundation package infos...");

                var currentAssembly = Assembly.GetEntryAssembly();
                var versionAttribute = currentAssembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                var request = WebRequest.CreateHttp(Url + versionAttribute.InformationalVersion);

                using var response = request.GetResponse();
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

        private static bool DownloadDependencies() => DownloadPackages("Download missing dependencies:", _absentPackages.ToArray());

        private static bool DownloadPackages(string task, IList<PackageInfo> packages)
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

        private static async Task DownloadPackage(string id, string version)
        {
            string destFile = Path.Combine(StagingPackagesDirectory, $"{id}.{version}.nupkg");
            if (localDebug)
            {
                string localPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
                    ".nuget", "packages", id, version, $"{id}.{version}.nupkg");
                if (File.Exists(localPath))
                {
                    File.Copy(localPath, destFile);
                    return;
                }
            }

            var request = WebRequest.CreateHttp($"https://api.nuget.org/v3-flatcontainer/{id}/{version}/{id}.{version}.nupkg");

            using var md5 = new MD5CryptoServiceProvider();
            using var response = await request.GetResponseAsync();
            var responseStream = response.GetResponseStream();
            var file = new FileInfo(destFile);
            var tempFilename = destFile + ".tmp";

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

            File.Move(tempFilename, destFile);
        }

        private static void ExtractPackages()
        {
            var stagingPackagesDirectory = new DirectoryInfo(StagingPackagesDirectory);
            var files = stagingPackagesDirectory.EnumerateFiles("*.nupkg");
            if (files.Any())
            {
                PrintLine("Extracting staging packages:");

                var lockingFilename = Path.Combine(StagingPackagesDirectory, ".lock");

                using (new FileStream(lockingFilename, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 4096, FileOptions.DeleteOnClose))
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

                PrintLine();
            }

            Directory.Delete(StagingPackagesDirectory, true);
        }

        private static PackageExtractionInfo ExtractPackage(FileInfo file)
        {
            const string ManifestRelationshipType = "http://schemas.microsoft.com/packaging/2010/07/manifest";

            try
            {
                using (var stream = file.OpenRead())
                {
                    var archive = new ZipArchive(stream);
                    var package = PackageContainer.Open(stream);
                    var identifier = package.PackageProperties.Identifier;
                    var directory = Path.Combine(Package.BaseDirectory, identifier);
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

        private static void ReplaceMyself(string directory)
        {
            var launcherFilename = Assembly.GetEntryAssembly().Location;
            var libDirectory = Path.Combine(directory, "lib");

            File.Move(launcherFilename, launcherFilename + ".old");
            File.Copy(Path.Combine(libDirectory, "Sakuno.ING.exe"), launcherFilename);

            var configFilename = Path.Combine(libDirectory, "Sakuno.ING.exe.config");
            var currentConfigFilename = launcherFilename + ".config";

            if (File.Exists(currentConfigFilename))
                File.Delete(currentConfigFilename);

            if (File.Exists(configFilename))
                File.Copy(configFilename, currentConfigFilename);

            needRestart = true;
        }

        private static void ExtractPackagePart(PackagePartInfo info, string packageDirectory, string filename)
        {
            var filepath = Path.Combine(packageDirectory, filename);

            Directory.CreateDirectory(Path.GetDirectoryName(filepath));

            using (var output = File.Create(filepath))
            using (var partStream = info.Part.GetStream())
                partStream.CopyTo(output);

            File.SetLastWriteTime(filepath, info.Entry.LastWriteTime.LocalDateTime);
        }
    }
}
