using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Proxy;
using System;
using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    public class ApiInfo
    {
        internal ApiSession Session { get; }

        public long Timestamp { get; }

        public string Api { get; }
        public IDictionary<string, string> Parameters { get; }
        public JObject Json { get; }

        public object Data { get; internal set; }

        internal ApiInfo(ApiSession rpSession, string rpApi, IDictionary<string, string> rpParameters, JObject rpJson)
        {
            Session = rpSession;

            Timestamp = DateTimeOffset.Now.ToUnixTime();

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
            if (rData != null)
                Data = rData.ToObject<T>();

            return Data as T;
        }
    }
}
