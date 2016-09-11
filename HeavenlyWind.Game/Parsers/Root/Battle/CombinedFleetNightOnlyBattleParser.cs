using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_combined_battle/sp_midnight")]
    class CombinedFleetNightOnlyBattleParser : ApiParser<RawCombinedFleetNightOnly>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawCombinedFleetNightOnly rpData)
        {
        }
    }
}
