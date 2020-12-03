using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public sealed class FleetCompositionChange
    {
        public FleetId FleetId { get; }
        public int? Index { get; }
        public ShipId? ShipId { get; }

        public FleetCompositionChange(FleetId fleetId, int? index, ShipId? shipId)
        {
            FleetId = fleetId;
            Index = index;
            ShipId = shipId;
        }
    }
}
