using System.Collections.Generic;

namespace Sakuno.ING.Game.Models
{
    public interface IRawFleet : IIdentifiable
    {
        string Name { get; }
        FleetExpeditionState ExpeditionState { get; }
        IReadOnlyList<int> ShipIds { get; }
    }
}
