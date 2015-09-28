using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services.Quest;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class QuestViewModel : ModelBase
    {
        public Quest Source { get; }

        public ProgressInfo Progress { get; private set; }

        internal QuestViewModel(Quest rpQuest)
        {
            Source = rpQuest;
        }
    }
}
