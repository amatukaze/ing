namespace Sakuno.ING.Game.Events
{
    public class CompositionChange
    {
        public CompositionChange(int fleetId, int? index, int? shipId)
        {
            FleetId = fleetId;
            Index = index;
            ShipId = shipId;
        }

        public int FleetId { get; }
        public int? Index { get; }
        public int? ShipId { get; }
    }
}
