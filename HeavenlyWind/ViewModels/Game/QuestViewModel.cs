using Sakuno.KanColle.Amatsukaze.Game.Models;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class QuestViewModel : ModelBase
    {
        public Quest Source { get; }

        internal QuestViewModel(Quest rpQuest)
        {
            Source = rpQuest;
        }
    }
}
