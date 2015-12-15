using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_sortie/battle")]
    class DayNormalBattleParser : ApiParser<RawDay>
    {
        public override void Process(RawDay rpData)
        {
        }
    }
}
