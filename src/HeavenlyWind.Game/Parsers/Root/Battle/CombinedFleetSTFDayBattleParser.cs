using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_combined_battle/each_battle_water")]
    class CombinedFleetSTFDayBattleParser : ApiParser<RawEnemyCombinedFleetDay>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawEnemyCombinedFleetDay rpData)
        {
        }
    }
}
