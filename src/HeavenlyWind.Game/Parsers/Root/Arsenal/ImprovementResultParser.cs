using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Arsenal
{
    [Api("api_req_kousyou/remodel_slot")]
    class ImprovementResultParser : ApiParser<RawImprovementResult>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawImprovementResult rpData)
        {
        }
    }
}
