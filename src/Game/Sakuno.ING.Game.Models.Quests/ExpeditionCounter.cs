using System;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models.Quests
{
    internal class ExpeditionCounter : QuestCounter
    {
        private readonly Predicate<ExpeditionId> expeditionFilter;

        public ExpeditionCounter(in QuestCounterParams @params, Predicate<ExpeditionId> expeditionFilter = null) : base(@params)
        {
            this.expeditionFilter = expeditionFilter;
        }

        public virtual void OnExpeditionComplete(IStatePersist statePersist, HomeportFleet fleet, ExpeditionInfo expedition, ExpeditionResult result)
        {
            if (result != ExpeditionResult.Fail &&
                expeditionFilter?.Invoke(expedition.Id) != false)
                Increase(statePersist);
        }
    }
}
