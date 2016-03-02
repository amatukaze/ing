using Sakuno.KanColle.Amatsukaze.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    public class QuestsViewModel : TabItemViewModel
    {
        public override string Name
        {
            get { return StringResources.Instance.Main.Tab_Quests; }
            protected set { throw new NotImplementedException(); }
        }

        IReadOnlyCollection<QuestViewModel> r_Executing;
        public IReadOnlyCollection<QuestViewModel> Executing
        {
            get { return r_Executing; }
            private set
            {
                if (r_Executing != value)
                {
                    r_Executing = value;
                    OnPropertyChanged(nameof(Executing));
                }
            }
        }
        IReadOnlyCollection<QuestViewModel> r_Unexecuted;
        public IReadOnlyCollection<QuestViewModel> Unexecuted
        {
            get { return r_Unexecuted; }
            private set
            {
                if (r_Unexecuted != value)
                {
                    r_Unexecuted = value;
                    OnPropertyChanged(nameof(Unexecuted));
                }
            }
        }

        internal QuestsViewModel(GameInformationViewModel rpParent)
        {
            var rQuestManager = KanColleGame.Current.Port.Quests;

            Executing = rpParent.Overview.ExecutingQuests = rQuestManager.Executing?.Select(r => new QuestViewModel(r)).ToList();
            Unexecuted = rQuestManager.Unexecuted?.Select(r => new QuestViewModel(r)).ToList();

            var rQuestManagerPCEL = PropertyChangedEventListener.FromSource(rQuestManager);
            rQuestManagerPCEL.Add(nameof(rQuestManager.Executing), (s, e) => Executing = rpParent.Overview.ExecutingQuests = rQuestManager.Executing.Select(r => new QuestViewModel(r)).ToList());
            rQuestManagerPCEL.Add(nameof(rQuestManager.Unexecuted), (s, e) => Unexecuted = rQuestManager.Unexecuted.Select(r => new QuestViewModel(r)).ToList());
        }
    }
}
