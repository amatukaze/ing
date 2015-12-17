using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Triggers;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest.Updaters;
using System;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Services.Quest
{
    public class ProgressRule
    {
        public Trigger Trigger { get; }
        public Updater Updater { get; }

        public ProgressRule(Trigger rpTrigger, Updater rpUpdater)
        {
            Trigger = rpTrigger;
            Updater = rpUpdater;
        }

        public void Register(QuestInfo rpQuest)
        {
            if (Trigger is UnknownTrigger)
                return;

            if (Trigger.Observable == null)
                throw null;

            Trigger.Observable.Subscribe(_ =>
            {
                if (QuestProgressService.Instance.Progresses == null)
                    return;

                ProgressInfo rProgressInfo;
                if (!QuestProgressService.Instance.Progresses.TryGetValue(rpQuest.ID, out rProgressInfo) || rProgressInfo.State != QuestState.Executing)
                    return;

                Updater.Invoke(rProgressInfo);
            });
        }

        public override string ToString() => $@"Trigger -> {Trigger}{Environment.NewLine}Updater -> {Updater}";
    }
}
