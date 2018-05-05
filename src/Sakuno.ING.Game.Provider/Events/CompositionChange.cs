namespace Sakuno.ING.Game.Events
{
    public class CompositionChange
    {
        public int FleetId { get; internal set; }
        public int? Index { get; internal set; }
        public int? ShipId { get; internal set; }
    }
}
