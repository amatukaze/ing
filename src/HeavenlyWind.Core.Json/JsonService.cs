using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Data.Json;
using System.IO;
using System.Threading.Tasks;

namespace Sakuno.KanColle.Amatsukaze.Services.Json
{
    class JsonService : IJsonService
    {
        public IJsonToken Parse(string json) =>
            JsonToken.Create(JToken.Parse(json));

        public IJsonToken Parse(Stream stream) =>
            JsonToken.Create(JToken.Load(new JsonTextReader(new StreamReader(stream))));
        public async Task<IJsonToken> ParseAsync(Stream stream) =>
            JsonToken.Create(await JToken.LoadAsync(new JsonTextReader(new StreamReader(stream))).ConfigureAwait(false));
    }
}
