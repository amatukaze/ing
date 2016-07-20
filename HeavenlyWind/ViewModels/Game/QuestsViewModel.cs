using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Views.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.ViewModels.Game
{
    [ViewInfo(typeof(Quests))]
    public class QuestsViewModel : TabItemViewModel
    {
        public override string Name
        {
            get { return StringResources.Instance.Main.Tab_Quests; }
            protected set { throw new NotImplementedException(); }
        }

        GameInformationViewModel r_Owner;

        public bool IsLoaded { get; private set; }

        IDTable<QuestViewModel> r_Quests = new IDTable<QuestViewModel>();
        QuestViewModel r_Dummy = new QuestViewModel(Quest.Dummy);

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

            var rQuestManager = KanColleGame.Current.Port.Quests;

            var rQuestManagerPCEL = PropertyChangedEventListener.FromSource(rQuestManager);
            rQuestManagerPCEL.Add(nameof(rQuestManager.IsLoaded), delegate
            {
                IsLoaded = true;
                OnPropertyChanged(nameof(IsLoaded));
            });

            rQuestManagerPCEL.Add(nameof(rQuestManager.Active), (s, e) => UpdateActiveQuests());
            UpdateActiveQuests();
        }

        void UpdateActiveQuests()
        {
            if (r_Quests.UpdateRawData(KanColleGame.Current.Port.Quests.Table.Values, r => new QuestViewModel(r), delegate { }))
            {
                All = r_Quests.Values.ToArray();
                OnPropertyChanged(nameof(All));

                var rQuestGroups = All.ToLookup(r => r.Source.Type);

                Daily = rQuestGroups[QuestType.Daily].ToArray();
                Weekly = rQuestGroups[QuestType.Weekly].ToArray();
                Monthly = rQuestGroups[QuestType.Monthly].ToArray();
                Once = rQuestGroups[QuestType.Once].ToArray();
                Others = rQuestGroups[QuestType.Special].ToArray();

                OnPropertyChanged(nameof(Daily));
                OnPropertyChanged(nameof(Weekly));
                OnPropertyChanged(nameof(Monthly));
                OnPropertyChanged(nameof(Once));
                OnPropertyChanged(nameof(Others));
            }

            var rActiveQuests = KanColleGame.Current.Port.Quests.Active;
            if (rActiveQuests != null)
            {
                Active = rActiveQuests.Select(r => r != Quest.Dummy ? r_Quests[r.ID] : r_Dummy).ToArray();
                r_Owner.Overview.ActiveQuests = Active;

                OnPropertyChanged(nameof(Active));
            }
        }
    }
}
