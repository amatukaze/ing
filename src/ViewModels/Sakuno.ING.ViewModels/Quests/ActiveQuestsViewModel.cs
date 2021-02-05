using DynamicData;
using ReactiveUI;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models.Quests;
using System.Collections.Generic;

namespace Sakuno.ING.ViewModels.Quests
{
    [Export]
    public sealed class ActiveQuestsViewModel : ReactiveObject
    {
        public IReadOnlyCollection<ActiveQuestViewModel> Quests { get; }

        public ActiveQuestsViewModel(QuestManager questManager)
        {
            Quests = questManager.Quests.DefaultViewSource
                .AutoRefresh(r => r.State).Filter(r => r.State == QuestState.Active)
                .Transform(r => new ActiveQuestViewModel(r)).Bind();
        }
    }
}
