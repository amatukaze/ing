using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_combined_battle/ld_airbattle")]
    class FriendCombinedFleetAerialAttackParser : ApiParser<RawCombinedFleetAerialAttack>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawCombinedFleetAerialAttack rpData)
        {
        }
    }
}
