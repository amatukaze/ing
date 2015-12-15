using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_combined_battle/battle")]
    class CombinedFleetCTFDayBattleParser : ApiParser<RawCombinedFleetDay>
    {
        public override void Process(RawCombinedFleetDay rpData)
        {
        }
    }
}
