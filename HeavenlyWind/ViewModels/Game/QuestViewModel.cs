using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class QuestViewModel : ModelBase
    {
        public Quest Source { get; }

        public QuestRealtimeProgressViewModel RealtimeProgress { get; }

        internal QuestViewModel(Quest rpQuest)
        {
            Source = rpQuest;

            RealtimeProgress = new QuestRealtimeProgressViewModel(rpQuest);
        }
    }
}
