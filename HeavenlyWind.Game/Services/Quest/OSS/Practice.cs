using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.OSS
{
    [Quest(303)]
    [Quest(304)]
    [Quest(302)]
    [Quest(307)]
    [Quest(308)]
    class Practice : OSSQuestProgressRule
    {
        public override void Register(QuestInfo rpQuest)
        {
            SessionService.Instance.Subscribe("api_req_practice/battle_result", r =>
            {
                ProgressInfo rProgressInfo;
                if (!QuestProgressService.Instance.Progresses.TryGetValue(rpQuest.ID, out rProgressInfo) || rProgressInfo.State != QuestState.Active)
                    return;

                if (r.GetData<RawBattleResult>().Rank >= BattleRank.B)
                    rProgressInfo.Progress++;
            });
        }
    }
}
