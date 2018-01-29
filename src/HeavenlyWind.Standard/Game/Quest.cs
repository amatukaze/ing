namespace Sakuno.KanColle.Amatsukaze.Game
{
    public sealed class Quest : IIdentifiable
    {
        public Quest
        (
            int id,
            QuestCategory category,
            QuestPeriod period,
            QuestState state,
            QuestBonus bonus,
            string title,
            string detail,
            int rewardFuel,
            int rewardBullet,
            int rewardSteel,
            int rewardBauxite
        )
        {
            Id = id;
            Category = category;
            Period = period;
            State = state;
            Bonus = bonus;
            Title = title;
            Detail = detail;
            RewardFuel = rewardFuel;
            RewardBullet = rewardBullet;
            RewardSteel = rewardSteel;
            RewardBauxite = rewardBauxite;
        }

        public int Id { get; }
        public QuestCategory Category { get; }
        public QuestPeriod Period { get; }
        public QuestState State { get; }
        public QuestProgress Progress { get; }
        public QuestBonus Bonus { get; }

        public string Title { get; }
        public string Detail { get; }
        public int RewardFuel { get; }
        public int RewardBullet { get; }
        public int RewardSteel { get; }
        public int RewardBauxite { get; }
    }

    public enum QuestCategory
    {
        Unknown = 0,
        Organization = 1,
        Sortie = 2,
        Exercise = 3,
        Expedition = 4,
        Charge = 5,
        Shipyard = 6,
        PowerUp = 7,
        Sortie2 = 8,
    }

    public enum QuestPeriod
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Once = 4,
        Other = 5,
    }

    public enum QuestState
    {
        None = 1,
        InProgress = 2,
        Completed = 3,
    }

    public enum QuestProgress
    {
        None = 0,
        Half = 1,
        Almost = 2,
    }

    public enum QuestBonus
    {
        None = 0,
        Normal = 1,
        Ship = 2,
    }
}
