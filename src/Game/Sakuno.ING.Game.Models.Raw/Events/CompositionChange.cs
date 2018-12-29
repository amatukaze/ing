using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public readonly struct CompositionChange
    {
        public CompositionChange(FleetId fleetId, int? index, ShipId? shipId)
        {
            FleetId = fleetId;
            Index = index;
            ShipId = shipId;
        }

        public FleetId FleetId { get; }
        public int? Index { get; }
        public ShipId? ShipId { get; }
    }
}
