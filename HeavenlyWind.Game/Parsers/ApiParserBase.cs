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

        public IReadOnlyDictionary<string, string> Requests { get; internal set; }

        public int ResultCode { get; private set; }
        public JObject ResponseJson { get; private set; }

        internal Subject<ApiData> ProcessSucceeded { get; } = new Subject<ApiData>();

        internal virtual void Process(JObject rpJson)
        {
            ResultCode = (int)rpJson["api_result"];
            ResponseJson = rpJson;
        }

        protected void OnProcessSucceeded(ApiData rpData) => ProcessSucceeded.OnNext(rpData);
    }
}
