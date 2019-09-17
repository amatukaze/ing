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
            var lastTime = owner.StatePersist.GetQuestTime(Id);
            owner.StatePersist.SetQuestActive(Id, State == QuestState.Active, timeStamp);
            Targets?.Check(timeStamp, lastTime, Period);
        }
    }
}
