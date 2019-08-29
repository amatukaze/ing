using System;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Quests
{
    internal class ExpeditionCounter : QuestCounter
    {
        private readonly Predicate<ExpeditionId> expeditionFilter;

        public ExpeditionCounter(QuestId questId, int maximum, Predicate<ExpeditionId> expeditionFilter = null, int counterId = 0) : base(questId, maximum, counterId)
        {
            this.expeditionFilter = expeditionFilter;
        }

        public ExpeditionCounter(QuestId questId, int maximum, ExpeditionId expeditionId, int counterId = 0) : base(questId, maximum, counterId)
        {
            expeditionFilter = e => e == expeditionId;
        }

        public virtual void OnExpeditionComplete(IStatePersist statePersist, HomeportFleet fleet, ExpeditionInfo expedition, ExpeditionResult result)
        {
            if (result != ExpeditionResult.Fail &&
                expeditionFilter?.Invoke(expedition.Id) != false)
                Increase(statePersist);
        }
    }
}
