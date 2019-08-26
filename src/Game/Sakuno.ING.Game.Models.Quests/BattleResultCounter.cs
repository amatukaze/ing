using Sakuno.ING.Game.Models.Combat;

namespace Sakuno.ING.Game.Models.Quests
{
    internal class BattleResultCounter : QuestCounter
    {
        public BattleResultCounter(IStatePersist statePersist, QuestId questId, int maximum, int counterId = 0) : base(statePersist, questId, maximum, counterId)
        {
        }

        public void OnBattleComplete(MapRouting routing, Battle battle, BattleResult result)
        {

        }
    }
}
