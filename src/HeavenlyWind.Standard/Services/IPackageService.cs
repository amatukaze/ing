using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Providers.Package;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public interface IPackageService
    {
        IReadOnlyList<IPackage> Modules { get; }

        IPackageProvider SelectedProvider { get; set; }
        IPackageProvider DefaultProvider { get; }

        Task<string> GetLastestVersionAsync(string packageId);

        Task<IPackage> FetchAndStageAsync(string packageId, string version);
    }
}
