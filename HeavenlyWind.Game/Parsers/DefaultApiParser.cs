using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Game.Proxy;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    internal sealed class DefaultApiParser : ApiParserBase
    {
        internal override sealed void Process(ApiSession rpSession, JObject rpJson)
        {
            base.Process(rpSession, rpJson);

            OnProcessSucceeded(new ApiData(rpSession, Api, Parameters, rpJson));
        }
    }
}
