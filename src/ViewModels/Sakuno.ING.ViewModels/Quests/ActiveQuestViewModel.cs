using ReactiveUI;
using Sakuno.ING.Game.Models.Quests;

namespace Sakuno.ING.ViewModels.Quests
{
    public sealed class ActiveQuestViewModel : ReactiveObject
    {
        public string Name { get; }

        public ActiveQuestViewModel(Quest quest)
        {
            Name = quest.Name;
        }
    }
}
