using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Parsers.Root.Member
{
    [Api("api_req_member/get_practice_enemyinfo")]
    class PracticeOpponentsParser : ApiParser<RawPracticeOpponentInfo>
    {
        public override void ProcessCore(ApiInfo rpInfo, RawPracticeOpponentInfo rpData)
        {
            if (Game.Practice == null)
                Game.Practice = new PracticeInfo(rpData);
            else
                Game.Practice.Opponent.Update(rpData);
        }
    }
}
