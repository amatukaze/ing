using Sakuno.ING.Game.Models.MasterData;
using System;

namespace Sakuno.ING.Game.Models
{
    public readonly struct RawFleetExpeditionStatus
    {
        public FleetExpeditionState State { get; }
        public ExpeditionId ExpeditionId { get; }
        public DateTimeOffset ReturnTime { get; }

        public RawFleetExpeditionStatus(FleetExpeditionState state, ExpeditionId expeditionId, DateTimeOffset returnTime)
        {
            State = state;
            ExpeditionId = expeditionId;
            ReturnTime = returnTime;
        }
    }
}
