using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw.Battle;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Battle
{
    [Api("api_req_practice/battle")]

    class PracticeDayBattleParser : DayNormalBattleParser
    {
        public override void ProcessCore(ApiInfo rpInfo, RawDay rpData)
        {
            var rPracticeInfo = Game.Sortie as PracticeInfo;
            if (rPracticeInfo != null)
            {
                var rParticipantFleet = Game.Port.Fleets[int.Parse(rpInfo.Parameters["api_deck_id"])];

                rPracticeInfo.Battle = new BattleInfo(rpInfo.Timestamp, rParticipantFleet);
            }
        }
    }
}
