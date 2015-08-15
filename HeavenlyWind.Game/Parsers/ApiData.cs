using Newtonsoft.Json.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    public class ApiData
    {
        public string Api { get; }
        public JObject Json { get; }

        public object Data { get; internal set; }

        internal ApiData(string rpApi, JObject rpJson)
        {
            Api = rpApi;
            Json = rpJson;
        }
    }
}
