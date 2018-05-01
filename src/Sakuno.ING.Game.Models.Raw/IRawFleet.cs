using System;
using System.Collections.Generic;

namespace Sakuno.ING.Game.Models
{
    public interface IRawFleet : IIdentifiable
    {
        string Name { get; }
        FleetExpeditionState ExpeditionState { get; }
        int ExpeditionId { get; }
        DateTimeOffset ExpeditionCompletionTime { get; }
        IReadOnlyList<int> ShipIds { get; }
    }
}
