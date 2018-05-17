using Sakuno.KanColle.Amatsukaze.Collections;
using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Views.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Data;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    [ViewInfo(typeof(Quests))]
    class QuestsViewModel : TabItemViewModel
    {
        public override string Name
        {
            get { return StringResources.Instance.Main.Tab_Quests; }
            protected set { throw new NotImplementedException(); }
        }

        GameInformationViewModel r_Owner;

        public bool IsLoaded { get; private set; }

        QuestViewModel r_Dummy = new QuestViewModel(Quest.Dummy);

        ProjectionCollection<Quest, QuestViewModel> _quests;
        ProjectionCollection<Quest, QuestViewModel> _activeQuests;

        public IList<QuestViewModel> All { get; private set; }
        public IList<QuestViewModel> Active { get; private set; }

        public IList<QuestViewModel> Daily { get; private set; }
        public IList<QuestViewModel> Weekly { get; private set; }
        public IList<QuestViewModel> Monthly { get; private set; }
        public IList<QuestViewModel> Once { get; private set; }
        public IList<QuestViewModel> Others { get; private set; }

        internal QuestsViewModel(GameInformationViewModel rpOwner)
        {
            r_Owner = rpOwner;

            _quests = new ProjectionCollection<Quest, QuestViewModel>(KanColleGame.Current.Port.Quests.Table, r => new QuestViewModel(r));
            _activeQuests = new ProjectionCollection<Quest, QuestViewModel>(KanColleGame.Current.Port.Quests.Active, r => new QuestViewModel(r));

            BindingOperations.EnableCollectionSynchronization(_quests, new object());
            BindingOperations.EnableCollectionSynchronization(_activeQuests, new object());

            var rQuestManager = KanColleGame.Current.Port.Quests;

            var rQuestManagerPCEL = PropertyChangedEventListener.FromSource(rQuestManager);
            rQuestManagerPCEL.Add(nameof(rQuestManager.IsLoaded), delegate
            {
                IsLoaded = true;
                OnPropertyChanged(nameof(IsLoaded));
            });

            ApiService.Subscribe("api_get_member/questlist", _ => UpdateQuestTypes());
        }

        void UpdateQuestTypes()
        {
            var rQuestGroups = _quests.ToLookup(r => r.Source.Type);

            All = _quests.ToArray();
            Daily = rQuestGroups[QuestType.Daily].ToArray();
            Weekly = rQuestGroups[QuestType.Weekly].ToArray();
            Monthly = rQuestGroups[QuestType.Monthly].ToArray();
            Once = rQuestGroups[QuestType.Once].ToArray();
            Others = rQuestGroups[QuestType.Special].ToArray();

            OnPropertyChanged(nameof(All));
            OnPropertyChanged(nameof(Daily));
            OnPropertyChanged(nameof(Weekly));
            OnPropertyChanged(nameof(Monthly));
            OnPropertyChanged(nameof(Once));
            OnPropertyChanged(nameof(Others));

            Active = _activeQuests.ToArray();
            OnPropertyChanged(nameof(Active));

            r_Owner.Overview.ActiveQuests = Active;
        }
    }
}
