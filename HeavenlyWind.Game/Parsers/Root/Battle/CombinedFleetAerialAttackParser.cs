using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_combined_battle/ld_airbattle")]
    class CombinedFleetAerialAttackParser : ApiParser<RawCombinedFleetAerialAttack>
    {
        public override void Process(RawCombinedFleetAerialAttack rpData)
        {
        }
    }
}
