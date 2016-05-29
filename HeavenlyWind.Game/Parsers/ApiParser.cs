using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Proxy;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    public abstract class ApiParser : ApiParserBase
    {
        internal override sealed void Process(ApiSession rpSession, JObject rpJson)
        {
            base.Process(rpSession, rpJson);

            Process();

            OnProcessSucceeded(new ApiData(rpSession, Api, Parameters, rpJson));
        }
        public abstract void Process();
    }
    public abstract class ApiParser<T> : ApiParserBase
    {
        internal override sealed void Process(ApiSession rpSession, JObject rpJson)
        {
            base.Process(rpSession, rpJson);

            var rApiData = rpJson["api_data"];
            if (rApiData != null)
            {
                var rData = rApiData.ToObject<T>();

                Process(rData);

                OnProcessSucceeded(new ApiData(rpSession, Api, Parameters, rpJson) { Data = rData });
            }
        }
        public abstract void Process(T rpData);
    }
}
