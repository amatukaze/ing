using Sakuno.KanColle.Amatsukaze.Composition;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public interface IPackageService
    {
        IList<IPackage> Modules { get; }

        Task<string> GetLastestVersionAsync(string packageId);

        Task<IPackage> FetchAndStageAsync(string packageId, string version);
    }
}
