using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_practice/battle_result")]
    class PracticeResultParser : ApiParser<RawBattleResult>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawBattleResult rpData)
        {
        }
    }
}
