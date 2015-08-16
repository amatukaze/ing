using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;

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

        public QuestManager()
        {
            ApiParserManager.Instance["api_get_member/questlist"].ProcessSucceeded += delegate
            {
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
