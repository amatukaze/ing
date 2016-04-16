using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    public class ApiData
    {
        public string Api { get; }
        public IDictionary<string, string> Parameters { get; }
        public JObject Json { get; }

        public object Data { get; internal set; }

        internal ApiData(string rpApi, IDictionary<string, string> rpParameters, JObject rpJson)
        {
            Api = rpApi;
            Parameters = rpParameters;
            Json = rpJson;
        }

        public T GetData<T>() where T : class
        {
            var rResult = Data as T;
            if (rResult != null)
                return rResult;

            var rData = Json["api_data"];
            return rData != null ? rData.ToObject<T>() : null;
        }
    }
}
