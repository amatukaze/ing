using System.IO;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Providers.Package
{
    public interface IPackageProvider
    {
        Task<Stream> FetchAsync(string id, string version);
    }
}
