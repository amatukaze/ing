using System.Collections.Generic;
using Sakuno.ING.Game.Models.Quests;

namespace Sakuno.ING.Game.Events
{
    public sealed class QuestPageUpdate
    {
        public QuestPageUpdate(int totalCount, bool anyCompleted, int pageCount, int pageId, int activeCount, IReadOnlyList<RawQuest> quests)
        {
            TotalCount = totalCount;
            AnyCompleted = anyCompleted;
            PageCount = pageCount;
            PageId = pageId;
            ActiveCount = activeCount;
            Quests = quests;
        }

        public int TotalCount { get; }
        public bool AnyCompleted { get; }
        public int PageCount { get; }
        public int PageId { get; }
        public int ActiveCount { get; }
        public IReadOnlyList<RawQuest> Quests { get; }
    }
}
