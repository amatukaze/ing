using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Models.Raw;
using System;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.OSS
{
    abstract class Battle : OSSQuestProgressRule
    {
        public override void Register(QuestInfo rpQuest)
        {
            var rBattleResultApis = new[]
            {
                "api_req_sortie/battleresult",
                "api_req_combined_battle/battleresult",
            };
            SessionService.Instance.Subscribe(rBattleResultApis, r =>
            {
                ProgressInfo rProgressInfo;
                if (!QuestProgressService.Instance.Progresses.TryGetValue(rpQuest.ID, out rProgressInfo) || rProgressInfo.State != QuestState.Executing)
                    return;

                Process(rProgressInfo, BattleInfo.Current, r.GetData<RawBattleResult>());

                rProgressInfo.Progress = Math.Min(rProgressInfo.Progress, rProgressInfo.Quest.Total);
            });
        }

        protected abstract void Process(ProgressInfo rpProgress, BattleInfo rpBattle, RawBattleResult rpResult);
    }
}
