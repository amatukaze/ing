using Sakuno.ING.Game.Models.Combat;

namespace Sakuno.ING.Game.Models.Quests
{
    internal class BattleResultCounter : QuestCounter
    {
        public BattleResultCounter(QuestId questId, int maximum, int counterId = 0) : base(questId, maximum, counterId)
        {
        }

        public void OnBattleComplete(MapRouting routing, Battle battle, BattleResult result)
        {

        }
    }
}
