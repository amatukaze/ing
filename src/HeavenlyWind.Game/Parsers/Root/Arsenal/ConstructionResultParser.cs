using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Arsenal
{
    [Api("api_req_kousyou/getship")]
    class ConstructionResultParser : ApiParser<RawConstructionResult>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawConstructionResult rpData)
        {
        }
    }
}
