using ReactiveUI;
using Sakuno.ING.Game.Models.Quests;

namespace Sakuno.ING.ViewModels.Quests
{
    public sealed class ActiveQuestViewModel : ReactiveObject
    {
        public Quest Model { get; }

        public ActiveQuestViewModel(Quest quest)
        {
            Model = quest;
        }
    }
}
