using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Services;

namespace Sakuno.ING.Bootstrap
{
    internal class PackageService : IPackageService
    {
        private readonly IPackageStorage _storage;

        public PackageService(IEnumerable<PackageStartupInfo> packages, IPackageStorage storage)
        {
            _storage = storage;
            InstalledPackages = packages.Select(p => new Package
            {
                Id = p.Id,
                Version = p.Version,
                IsMetaPackage = p.Module == null,
                IsLoaded = true,
                IsEnabled = true,
                Dependencies = p.Dependencies,
            }).ToArray();
        }

        public IReadOnlyList<IPackage> InstalledPackages { get; }
        public IPackageProvider SelectedProvider { get; set; }
        public bool IsReadOnly => _storage != null;
    }
}
