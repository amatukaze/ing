namespace Sakuno.ING.Game.Models.Quests
{
    public abstract class QuestCounter : BindableObject
    {
        private readonly IStatePersist statePersist;
        private readonly QuestId questId;
        private readonly int counterId;

        protected QuestCounter(IStatePersist statePersist, QuestId questId, int maximum, int counterId = 0)
        {
            this.statePersist = statePersist;
            this.questId = questId;
            this.counterId = counterId;
            Progress = (statePersist.GetQuestProgress(questId, counterId) ?? 0, maximum);
        }

        private ClampedValue _progress;
        public ClampedValue Progress
        {
            get => _progress;
            private set => Set(ref _progress, value);
        }

        protected void Increase()
        {
            Progress += 1;
            statePersist.SetQuestProgress(questId, counterId, Progress.Current);
        }
    }
}
