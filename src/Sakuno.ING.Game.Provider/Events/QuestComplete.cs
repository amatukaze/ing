namespace Sakuno.ING.Game.Events
{
    public readonly struct QuestComplete
    {
        public readonly int QuestId;

        public QuestComplete(int questId) => QuestId = questId;
    }
}
