using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_sortie/airbattle")]
    class AerialCombatParser : ApiParser<RawAerialCombat>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawAerialCombat rpData)
        {
        }
    }
}
