using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using System.Collections.Generic;
using System.Linq;

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
        int r_ExecutingCount;
        public int ExecutingCount
        {
            get { return r_ExecutingCount; }
            internal set
            {
                if (r_ExecutingCount != value)
                {
                    r_ExecutingCount = value;
                    OnPropertyChanged(nameof(ExecutingCount));
                }
            }
        }

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

        IReadOnlyCollection<Quest> r_Executing;
        public IReadOnlyCollection<Quest> Executing
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
        IReadOnlyCollection<Quest> r_Unexecuted;
        public IReadOnlyCollection<Quest> Unexecuted
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
        
        internal QuestManager()
        {
            ApiParserManager.Instance["api_get_member/questlist"].ProcessSucceeded += delegate
            {
                var rQuests = Table.Values.ToLookup(r => r.State != QuestState.None);
                var rExecuting = rQuests[true].ToList();
                Unexecuted = rQuests[false].ToList();

                if (rExecuting.Count < ExecutingCount)
                    rExecuting.AddRange(Enumerable.Repeat<Quest>(Quest.Dummy, ExecutingCount - rExecuting.Count));

                Executing = rExecuting;

                IsLoaded = true;
                OnPropertyChanged(nameof(IsLoaded));
            };
            ApiParserManager.Instance["api_req_quest/clearitemget"].ProcessSucceeded += (rpRequests, _) =>
            {
                var rQuestID = int.Parse(rpRequests["api_quest_id"]);
                Table.Remove(rQuestID);
                TotalCount--;
            };
        }
    }
}
