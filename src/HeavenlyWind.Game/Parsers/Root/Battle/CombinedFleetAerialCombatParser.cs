using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_combined_battle/airbattle")]
    class CombinedFleetAerialCombatParser : ApiParser<RawCombinedFleetAerialCombat>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawCombinedFleetAerialCombat rpData)
        {
        }
    }
}
