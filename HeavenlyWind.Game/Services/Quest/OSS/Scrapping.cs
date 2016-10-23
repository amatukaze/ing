using Sakuno.KanColle.Amatsukaze.Game.Models;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest.OSS
{
    [Quest(638)]
    class Scrapping : OSSQuestProgressRule
    {
        public override void Register(QuestInfo rpQuest)
        {
            ApiService.SubscribeOnlyOnBeforeProcessStarted("api_req_kousyou/destroyitem2", rpInfo =>
            {
                ProgressInfo rProgressInfo;
                if (rpQuest.ID != 638 ||!QuestProgressService.Instance.Progresses.TryGetValue(rpQuest.ID, out rProgressInfo) || rProgressInfo.State != QuestState.Active)
                    return;

                rProgressInfo.Progress += rpInfo.Parameters["api_slotitem_ids"].Split(',')
                    .Select(r => KanColleGame.Current.Port.Equipment[int.Parse(r)]).Where(r => r.Info.Type == EquipmentType.AAGun).Count();
            });
        }
    }
}
