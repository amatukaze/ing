using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest;
using Sakuno.UserInterface.Commands;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class QuestRealtimeProgressViewModel : ModelBase
    {
        Quest r_Quest;

        public ICommand DecreaseCommand { get; }
        public ICommand IncreaseCommand { get; }

        public QuestRealtimeProgressViewModel(Quest rpQuest)
        {
            r_Quest = rpQuest;

            DecreaseCommand = new DelegatedCommand(Decrease, CanDecrease);
            IncreaseCommand = new DelegatedCommand(Increase, CanIncrease);
        }

        bool CanDecrease() => r_Quest.RealtimeProgress != null && r_Quest.RealtimeProgress.Progress > 0;
        void Decrease()
        {
            r_Quest.RealtimeProgress.Progress--;
        }

        bool CanIncrease() => r_Quest.RealtimeProgress == null || r_Quest.RealtimeProgress.Quest.Total == -1 || r_Quest.RealtimeProgress.Progress <= r_Quest.RealtimeProgress.Quest.Total;
        void Increase()
        {
            var rProgress = r_Quest.RealtimeProgress;
            if (rProgress == null)
            {
                RecordService.Instance.QuestProgress.InsertRecord(r_Quest.RawData, 0);
                rProgress = new ProgressInfo(r_Quest.ID, r_Quest.Type, QuestState.Active, 0);
                QuestProgressService.Instance.Progresses.Add(r_Quest.ID, rProgress);
                r_Quest.RealtimeProgress = rProgress;
            }

            rProgress.Progress++;
        }
    }
}
