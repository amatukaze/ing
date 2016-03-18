using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    public class ApiData
    {
        public string Api { get; }
        public IDictionary<string, string> Requests { get; }
        public JObject Json { get; }

        public object Data { get; internal set; }

        internal ApiData(string rpApi, IDictionary<string, string>  rpRequests, JObject rpJson)
        {
            Api = rpApi;
            Requests = rpRequests;
            Json = rpJson;
        }

        public T GetData<T>() where T : class
        {
            var rData = Json["api_data"];
            return rData != null ? rData.ToObject<T>() : null;
        }
    }
}
