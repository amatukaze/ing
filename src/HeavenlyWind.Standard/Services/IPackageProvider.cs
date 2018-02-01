using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Sakuno.KanColle.Amatsukaze.Composition;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public interface IPackageProvider
    {
        IReadOnlyList<IModuleMetadata> AvaiablePackages { get; }

        Task<string> GetLastestVersionAsync(string packageId);

        Task<IModuleMetadata> GetMetadataAsync(string id, string version);

        Task<Stream> FetchAsync(string id, string version);
    }
}
