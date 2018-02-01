using System;
using System.Linq;
using System.Threading.Tasks;
using Sakuno.KanColle.Amatsukaze.Services;

namespace Sakuno.KanColle.Amatsukaze.Bootstrap
{
    class App
    {
        IPackageService _packageService;
        IPackageProvider _packageProvider;

        public App(IPackageService packageService, IPackageProvider packageProvider)
        {
            _packageService = packageService;
            _packageProvider = packageProvider;
        }

        public void Run()
        {
            ListInstalledModules();

            CheckForModuleUpdates();

            Console.ReadKey();
        }

        void ListInstalledModules()
        {
            Console.WriteLine("Installed modules:");

            foreach (var package in _packageService.InstalledPackages)
            {
                Console.Write(" - ");
                Console.Write(package.Id);
                Console.Write(' ');
                Console.WriteLine(package.Version);
            }

            Console.WriteLine();
        }

        bool CheckForModuleUpdates()
        {
            Console.WriteLine("Checking for module updates...");

            var tasks = _packageService.InstalledPackages.Select(r => _packageProvider.GetLastestVersionAsync(r.Id)).ToArray();

            Task.WaitAll(tasks);

            var isAvailable = false;

            for (var i = 0; i < tasks.Length; i++)
            {
                var package = _packageService.InstalledPackages[i];
                var lastestVersion = tasks[i].Result;

                if (package.Version != lastestVersion)
                {
                    if (!isAvailable)
                    {
                        isAvailable = true;

                        Console.WriteLine("Following update(s) available:");
                    }

                    Console.Write(" - ");
                    Console.Write(package.Id);
                    Console.Write(' ');
                    Console.WriteLine(lastestVersion);
                }
            }

            if (!isAvailable)
                Console.WriteLine("No update available.");

            Console.WriteLine();

            return isAvailable;
        }
    }
}
