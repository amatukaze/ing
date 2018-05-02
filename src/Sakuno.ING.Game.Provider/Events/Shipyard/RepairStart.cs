namespace Sakuno.ING.Game.Events.Shipyard
{
    public class RepairStart
    {
        public bool InstantRepair { get; internal set; }
        public int ShipId { get; internal set; }
        public int RepairingDockId { get; internal set; }
    }
}
