using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_practice/battle")]

    class PracticeDayBattleParser : DayNormalBattleParser
    {
        public override void ProcessCore(ApiInfo rpInfo, RawDay rpData)
        {
            if (Game.Practice == null)
                return;

            var rParticipantFleet = Game.Port.Fleets[int.Parse(rpInfo.Parameters["api_deck_id"])];

            Game.Practice.Battle = new BattleInfo(rpInfo.Timestamp, rParticipantFleet);
        }
    }
}
