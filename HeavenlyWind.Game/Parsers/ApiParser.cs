using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    public abstract class ApiParser : ApiParserBase
    {
        internal override sealed void Process(JObject rpJson)
        {
            base.Process(rpJson);
            Process();

            OnProcessSucceeded(new ApiData(Api, rpJson));
        }
        public abstract void Process();
    }
    public abstract class ApiParser<T> : ApiParserBase
    {
        internal override sealed void Process(JObject rpJson)
        {
            base.Process(rpJson);
            try
            {
                T rData = default(T);

                var rApiData = rpJson["api_data"];
                if (rApiData != null)
                    rData = rApiData.ToObject<T>();

                Process(rData);

                OnProcessSucceeded(new ApiData(Api, rpJson) { Data = rData });
            }
            catch (JsonReaderException) { }
        }
        public abstract void Process(T rpData);
    }
}
