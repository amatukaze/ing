using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_sortie/ld_airbattle")]
    class AerialAttackParser : ApiParser<RawAerialAttack>
    {
        public override void Process(RawAerialAttack rpData)
        {
        }
    }
}
