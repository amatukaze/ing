using Sakuno.KanColle.Amatsukaze.Data.Json;
using System.IO;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public interface IJsonService
    {
        IJsonToken Parse(string json);
        IJsonToken Parse(Stream stream);

        Task<IJsonToken> ParseAsync(Stream stream);
    }
}
