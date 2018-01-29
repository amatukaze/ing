using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Bootstrap
{
    class PackageService : IPackageService
    {
        IDictionary<string, ModuleInfo> _modules;

        public IList<IPackage> Modules { get; }

        ICommonDirectoryService _commonDirectoryService;

        public PackageService(IModuleList moduleList, ICommonDirectoryService commonDirectoryService)
        {
            _modules = moduleList.Modules;
            Modules = _modules.Values.ToArray();

            _commonDirectoryService = commonDirectoryService;
        }

        public Task<string> GetLastestVersionAsync(string packageId)
        {
            throw new NotImplementedException();
        }

        public Task<IPackage> FetchAndStageAsync(string packageId, string version)
        {
            throw new NotImplementedException();
        }
    }
}
