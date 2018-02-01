using System.IO;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Bootstrap
{
    public interface IPackageStorage
    {
        Task StageAsync(string id, string version, Stream stream);

        void Remove(string id, string version);
    }
}
