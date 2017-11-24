using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_combined_battle/ec_night_to_day")]
    class EnemyCombinedFleetNightToDayBattleParser : ApiParser<RawEnemyCombinedFleetNightToDay>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawEnemyCombinedFleetNightToDay rpData)
        {
        }
    }
}
