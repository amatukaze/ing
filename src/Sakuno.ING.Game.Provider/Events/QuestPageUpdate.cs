using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public class QuestPageUpdate
    {
        public int TotalCount { get; internal set; }
        public bool AnyCompleted { get; internal set; }
        public int PageCount { get; internal set; }
        public int PageId { get; internal set; }
        public int ActiveCount { get; internal set; }
        public IReadOnlyList<IRawQuest> Quests { get; internal set; }
    }
}
