using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Proxy;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    public abstract class ApiParserBase
    {
        public KanColleGame Game => KanColleGame.Current;

        public string Api { get; internal set; }

        public IDictionary<string, string> Parameters { get; internal set; }

        internal Subject<ApiData> Observable { get; } = new Subject<ApiData>();

        internal virtual void Process(ApiSession rpSession, JObject rpJson)
        {
            var rResultCode = (int)rpJson["api_result"];
            if (rResultCode != 1)
                throw new ApiFailedException(rResultCode);
        }

        protected void OnProcessSucceeded(ApiData rpData) => Observable.OnNext(rpData);
    }
}
