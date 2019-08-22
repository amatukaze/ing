namespace Sakuno.ING.Game.Models.Quests
{
    public partial class Quest
    {
        public QuestTarget Targets { get; private set; }

        partial void CreateCore() => Targets = owner.Knowledges?.GetTargetFor(Id);
    }
}
