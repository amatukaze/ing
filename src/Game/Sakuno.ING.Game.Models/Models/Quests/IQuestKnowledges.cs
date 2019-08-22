namespace Sakuno.ING.Game.Models.Quests
{
    public interface IQuestKnowledges
    {
        QuestTarget GetTargetFor(QuestId id);
    }
}
