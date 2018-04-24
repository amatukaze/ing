using System.Collections.Generic;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public interface IRawFleet : IIdentifiable
    {
        string Name { get; }
        FleetExpeditionState ExpeditionState { get; }
        IReadOnlyList<int> ShipIds { get; }
    }
}
