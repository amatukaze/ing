using Sakuno.KanColle.Amatsukaze.Game;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

            var rPropertyChangedSource = Observable.FromEventPattern<PropertyChangedEventArgs>(rQuestManager, nameof(rQuestManager.PropertyChanged))
                .Select(r => r.EventArgs.PropertyName);
            rPropertyChangedSource.Where(r => r == nameof(rQuestManager.Executing))
                .Subscribe(_ => Executing = rpParent.Overview.ExecutingQuests = rQuestManager.Executing.Select(r => new QuestViewModel(r)).ToList());
            rPropertyChangedSource.Where(r => r == nameof(rQuestManager.Unexecuted))
                .Subscribe(_ => Unexecuted = rQuestManager.Unexecuted.Select(r => new QuestViewModel(r)).ToList());
        }
    }
}
