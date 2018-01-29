using Sakuno.KanColle.Amatsukaze.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Bootstrap
{
    class App
    {
        IPackageService _packageService;

        public App(IPackageService packageService)
        {
            _packageService = packageService;
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

            foreach (var module in _packageService.Modules)
            {
                Console.Write(" - ");
                Console.Write(module.Id);
                Console.Write(' ');
                Console.WriteLine(module.Version);
            }

            Console.WriteLine();
        }

        bool CheckForModuleUpdates()
        {
            Console.WriteLine("Checking for module updates...");

            var tasks = _packageService.Modules.Select(r => _packageService.GetLastestVersionAsync(r.Id)).ToArray();

            Task.WaitAll(tasks);

            var isAvailable = false;

            for (var i = 0; i < tasks.Length; i++)
            {
                var module = _packageService.Modules[i];
                var lastestVersion = tasks[i].Result;

                if (module.Version != lastestVersion)
                {
                    if (!isAvailable)
                    {
                        isAvailable = true;

                        Console.WriteLine("Following update(s) available:");
                    }

                    Console.Write(" - ");
                    Console.Write(module.Id);
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
