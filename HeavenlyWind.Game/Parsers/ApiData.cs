using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    public class ApiData
    {
        public string Api { get; }
        public IReadOnlyDictionary<string, string> Requests { get; }
        public JObject Json { get; }

        public object Data { get; internal set; }

        internal ApiData(string rpApi, IReadOnlyDictionary<string, string>  rpRequests, JObject rpJson)
        {
            Api = rpApi;
            Requests = rpRequests;
            Json = rpJson;
        }
    }
}
