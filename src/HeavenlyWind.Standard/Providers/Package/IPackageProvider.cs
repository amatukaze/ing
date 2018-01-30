using System.IO;
using System.Threading.Tasks;
using Sakuno.KanColle.Amatsukaze.Composition;

namespace Sakuno.KanColle.Amatsukaze.Providers.Package
{
    public interface IPackageProvider
    {
        Task<string> GetLastestVersionAsync(string packageId);

        Task<PackageMetadata> GetMetadataAsync(string id, string version);

        Task<Stream> FetchAsync(string id, string version);
    }
}
