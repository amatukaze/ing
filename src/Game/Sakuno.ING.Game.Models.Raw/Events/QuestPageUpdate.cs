using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public sealed class QuestPageUpdate
    {
        public QuestPageUpdate(int totalCount, bool anyCompleted, int pageCount, int pageId, int activeCount, IReadOnlyList<IRawQuest> quests)
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
        public IReadOnlyList<IRawQuest> Quests { get; }
    }
}
