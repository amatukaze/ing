using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Sakuno.KanColle.Amatsukaze.Game
{
    public class QuestManager : ModelBase
    {
        public IDTable<Quest> Table { get; } = new IDTable<Quest>();

        int r_TotalCount;
        public int TotalCount
        {
            get { return r_TotalCount; }
            internal set
            {
                if (r_TotalCount != value)
                {
                    r_TotalCount = value;
                    OnPropertyChanged(nameof(TotalCount));
                }
            }
        }

        public int ActiveQuestCount { get; internal set; }

        bool r_IsLoaded;
        public bool IsLoaded
        {
            get { return r_IsLoaded; }
            internal set
            {
                if (r_IsLoaded != value)
                {
                    r_IsLoaded = value;
                    OnPropertyChanged(nameof(IsLoaded));
                }
            }
        }

        public bool IsLoadCompleted => IsLoaded && TotalCount == Table.Count;

        public Quest this[int rpID] => Table[rpID];

        ObservableCollection<Quest> _activeQuests;
        public IReadOnlyList<Quest> Active { get; }

        internal QuestManager()
        {
            _activeQuests = new ObservableCollection<Quest>();
            Active = new ReadOnlyObservableCollection<Quest>(_activeQuests);

            ApiService.Subscribe("api_get_member/questlist", _ =>
            {
                UpdateQuestList();

                IsLoaded = true;
                OnPropertyChanged(nameof(IsLoaded));
            });
            ApiService.Subscribe("api_req_quest/stop", r =>
            {
                var rQuestID = int.Parse(r.Parameters["api_quest_id"]);
                Table[rQuestID].RawData.State = QuestState.None;
            });
            ApiService.Subscribe("api_req_quest/clearitemget", r =>
            {
                var rQuestID = int.Parse(r.Parameters["api_quest_id"]);
                Table.Remove(rQuestID);
                TotalCount--;
            });
        }

        internal void UpdateQuestList()
        {
            _activeQuests.Clear();

            foreach (var activeQuest in Table.Values.OrderBy(r => r.ID).Where(r => r.State != QuestState.None))
                _activeQuests.Add(activeQuest);

            var count = _activeQuests.Count;

            if (count < ActiveQuestCount)
                for (var i = 0; i < ActiveQuestCount - count; i++)
                    _activeQuests.Add(Quest.Dummy);
        }
    }
}
