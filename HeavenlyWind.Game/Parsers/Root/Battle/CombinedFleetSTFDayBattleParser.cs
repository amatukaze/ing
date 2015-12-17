using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_combined_battle/battle_water")]
    class CombinedFleetSTFDayBattleParser : ApiParser<RawCombinedFleetDay>
    {
        public override void Process(RawCombinedFleetDay rpData)
        {
        }
    }
}
