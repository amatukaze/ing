using System;

namespace Sakuno.ING.Game.Models.Quests
{
    public partial class Quest
    {
        public QuestTarget Targets { get; private set; }
        public bool HasTargets => Targets != null;

        partial void CreateCore() => Targets = owner.Knowledges?.GetTargetFor(Id);

        partial void UpdateCore(RawQuest raw, DateTimeOffset timeStamp)
        {
            owner.StatePersist.SetQuestActive(Id, State == QuestState.Active);
            Targets?.Check(timeStamp, Period);
        }
    }
}
