namespace Sakuno.ING.Game.Models.Quests
{
    public abstract class QuestCounter : BindableObject
    {
        private readonly QuestId questId;
        private readonly int maximum;
        private readonly int counterId;

        protected QuestCounter(QuestId questId, int maximum, int counterId = 0)
        {
            this.questId = questId;
            this.maximum = maximum;
            this.counterId = counterId;
        }

        private ClampedValue _progress;
        public ClampedValue Progress
        {
            get => _progress;
            private set => Set(ref _progress, value);
        }

        protected void Load(IStatePersist statePersist)
            => Progress = (statePersist.GetQuestProgress(questId, counterId) ?? 0, maximum);

        protected void Increase(IStatePersist statePersist)
        {
            Progress += 1;
            statePersist.SetQuestProgress(questId, counterId, Progress.Current);
        }
    }
}
