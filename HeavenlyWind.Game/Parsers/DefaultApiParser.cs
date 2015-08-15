using Newtonsoft.Json.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers
{
    public sealed class DefaultApiParser : ApiParserBase
    {
        internal override sealed void Process(JObject rpJson)
        {
            base.Process(rpJson);

            OnProcessSucceeded(new ApiData(Api, rpJson));
        }
    }
}
