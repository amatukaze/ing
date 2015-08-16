using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    public sealed class ApiParserManager
    {
        public static ApiParserManager Instance { get; } = new ApiParserManager();

        Dictionary<string, ApiParserBase> r_Parsers = new Dictionary<string, ApiParserBase>();

        public ApiParserBase this[string rpApi]
        {
            get
            {
                ApiParserBase rResult;
                if (!r_Parsers.TryGetValue(rpApi, out rResult))
                    r_Parsers.Add(rpApi, rResult = new DefaultApiParser() { Api = rpApi });

                return rResult;
            }
        }

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

                r_Parsers.Add(rAttribute.Name, rParser);
            }
        }

        public void Process(string rpApi, string rpRequest, string rpResponse)
        {
            var rContent = rpResponse.Replace("svdata=", string.Empty);

            ApiParserBase rParser;
            if (!rContent.IsNullOrEmpty() && rContent.StartsWith("{") && r_Parsers.TryGetValue(rpApi, out rParser))
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
