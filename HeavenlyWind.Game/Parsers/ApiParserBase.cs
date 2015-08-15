using Newtonsoft.Json.Linq;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    public abstract class ApiParserBase
    {
        public KanColleGame Game => KanColleGame.Current;

        public string Api { get; internal set; }

        public int ResultCode { get; private set; }
        public JObject ResponseJson { get; private set; }

        public event Action<ApiData> ProcessSucceeded = delegate { };

        internal virtual void Process(JObject rpJson)
        {
            ResultCode = (int)rpJson["api_result"];
            ResponseJson = rpJson;
        }

        protected void OnProcessSucceeded(ApiData rpData) => ProcessSucceeded(rpData);
    }
}
