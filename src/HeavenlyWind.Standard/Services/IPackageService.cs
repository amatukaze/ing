using System.Collections.Generic;
using Sakuno.KanColle.Amatsukaze.Composition;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public interface IPackageService
    {
        IReadOnlyList<IPackage> InstalledPackages { get; }

        IPackageProvider SelectedProvider { get; set; }

        bool IsReadOnly { get; }
    }
}
