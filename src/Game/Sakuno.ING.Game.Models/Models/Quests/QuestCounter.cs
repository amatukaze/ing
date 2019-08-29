using System;

namespace Sakuno.ING.Game.Models.Quests
{
    public abstract class QuestCounter : BindableObject
    {
        private readonly QuestId questId;
        private readonly int maximum;
        private readonly int counterId;
        private readonly QuestPeriod? periodOverride;

        protected QuestCounter(QuestId questId, int maximum, int counterId = 0, QuestPeriod? periodOverride = null)
        {
            this.questId = questId;
            this.maximum = maximum;
            this.counterId = counterId;
            this.periodOverride = periodOverride;
        }

        private ClampedValue _progress;
        public ClampedValue Progress
        {
            get => _progress;
            private set => Set(ref _progress, value);
        }

        public void Load(IStatePersist statePersist)
            => Progress = (statePersist.GetQuestProgress(questId, counterId) ?? 0, maximum);

        protected void Increase(IStatePersist statePersist, int increasedBy = 1)
        {
            if (increasedBy != 0 && statePersist.GetQuestActive(questId))
            {
                Progress += increasedBy;
                statePersist.SetQuestProgress(questId, counterId, Progress.Current);
            }
        }

        public void Check(IStatePersist statePersist, DateTimeOffset timeStamp, QuestPeriod period)
        {
            if (statePersist.GetQuestTime(questId, counterId) is DateTimeOffset t &&
                QuestManager.QuestExpires(t, timeStamp, periodOverride ?? period))
            {
                Progress = (0, maximum);
                statePersist.SetQuestProgress(questId, counterId, 0);
            }
            statePersist.SetQuestTime(questId, counterId, timeStamp);
        }
    }
}
