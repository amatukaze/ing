namespace Sakuno.ING.Game.Models
{
    public interface IRawQuest : IIdentifiable
    {
        QuestCategoty Category { get; }
        QuestPeriod Period { get; }
        QuestState State { get; }
        QuestProgress Progress { get; }
        string Name { get; }
        string Description { get; }
        Materials Rewards { get; }
    }

    public enum QuestCategoty
    {
        Composition = 1,
        Sortie = 2,
        Exercise = 3,
        Expedition = 4,
        Supply = 5,
        Arsenal = 6,
        Mordenization = 7
    }

    public enum QuestPeriod
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3,
        Once = 4,
        Quarterly = 5
    }

    public enum QuestState
    {
        Inactive = 1,
        Active = 2,
        Completed = 3
    }

    public enum QuestProgress
    {
        None = 0,
        Half = 1,
        Almost = 2
    }
}
