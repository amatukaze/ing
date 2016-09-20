using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Member
{
    [Api("api_req_member/get_practice_enemyinfo")]
    class PracticeOpponentsParser : ApiParser<RawPracticeOpponentInfo>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawPracticeOpponentInfo rpData)
        {
            var rPracticeInfo = Game.Sortie as PracticeInfo;
            if (rPracticeInfo != null)
                rPracticeInfo.Opponent.Update(rpData);
            else
                Game.Sortie = new PracticeInfo(rpData);
        }
    }
}
