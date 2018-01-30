using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sakuno.KanColle.Amatsukaze.Composition;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public interface IPackageService
    {
        IReadOnlyList<IPackage> Modules { get; }

        bool CanUpdate { get; }

        Task<IPackage> StageAsync(PackageMetadata metadata, Stream stream);
    }
}
