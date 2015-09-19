using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Reflection;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    public sealed class ApiParserManager
    {
        public static ApiParserManager Instance { get; } = new ApiParserManager();

        internal Dictionary<string, ApiParserBase> Parsers { get; } = new Dictionary<string, ApiParserBase>();

        Subject<ApiSession> r_SessionSources = new Subject<ApiSession>();

        ApiParserManager()
        {
            var rAssembly = Assembly.GetExecutingAssembly();

            var rParserTypes = rAssembly.GetTypes().Where(r => !r.IsAbstract && r.IsSubclassOf(typeof(ApiParserBase)));
            foreach (var rType in rParserTypes)
            {
                var rAttribute = rType.GetCustomAttribute<ApiAttribute>();
                if (rAttribute == null)
                    continue;

                var rParser = (ApiParserBase)Activator.CreateInstance(rType);
                rParser.Api = rAttribute.Name;

                Parsers.Add(rAttribute.Name, rParser);
            }

            r_SessionSources.Subscribe(r => Process(r.DisplayUrl, r.RequestBodyString, r.ResponseBodyString));
        }

        public void Process(ApiSession rpSession) => r_SessionSources.OnNext(rpSession);
        public void Process(string rpApi, string rpRequest, string rpResponse)
        {
            var rContent = rpResponse.Replace("svdata=", string.Empty);

            ApiParserBase rParser;
            if (!rContent.IsNullOrEmpty() && rContent.StartsWith("{") && Parsers.TryGetValue(rpApi, out rParser))
            {
                Dictionary<string, string> rRequests = null;
                if (!rpRequest.IsNullOrEmpty() && rpRequest.Contains('&'))
                    rRequests = rpRequest.Split('&').Where(r => r.Length > 0).Select(r => r.Split('='))
                        .ToDictionary(r => Uri.UnescapeDataString(r[0]), r => Uri.UnescapeDataString(r[1]));

                var rJson = JObject.Parse(rContent);

                rParser.Requests = rRequests;
                rParser.Process(rJson);
                rParser.Requests = null;
            }
        }
    }
}
