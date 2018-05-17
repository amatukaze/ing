using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_combined_battle/ec_midnight_battle")]
    class EnemyCombinedFleetNightBattleParser : ApiParser<RawEnemyCombinedFleetNight>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawEnemyCombinedFleetNight rpData)
        {
        }
    }
}
