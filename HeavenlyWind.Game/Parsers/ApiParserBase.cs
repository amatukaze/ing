using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    public abstract class ApiParserBase
    {
        public KanColleGame Game => KanColleGame.Current;

        public string Api { get; internal set; }

        public IDictionary<string, string> Requests { get; internal set; }

        internal Subject<ApiData> ProcessSucceeded { get; } = new Subject<ApiData>();

        internal virtual void Process(JObject rpJson)
        {
            var rResultCode = (int)rpJson["api_result"];
            if (rResultCode != 1)
                throw new ApiFailedException(rResultCode);
        }

        protected void OnProcessSucceeded(ApiData rpData) => ProcessSucceeded.OnNext(rpData);
    }
}
