using Sakuno.ING.Game.Models.Combat;

namespace Sakuno.ING.Game.Models.Quests
{
    public interface IQuestKnowledges
    {
        void Load();

        QuestTarget GetTargetFor(QuestId id);

        void OnBattleComplete(MapRouting routing, Battle battle, BattleResult result);
    }
}
