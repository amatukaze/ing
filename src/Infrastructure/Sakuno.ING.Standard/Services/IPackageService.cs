using System.Collections.Generic;
using Sakuno.ING.Composition;

namespace Sakuno.ING.Services
{
    public interface IPackageService
    {
        IReadOnlyList<IPackage> InstalledPackages { get; }

        IPackageProvider SelectedProvider { get; set; }

        bool IsReadOnly { get; }
    }
}
