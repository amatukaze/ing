using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_sortie/night_to_day")]
    class NightToDayBattleParser : ApiParser<RawNightToDay>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawNightToDay rpData)
        {
        }
    }
}
