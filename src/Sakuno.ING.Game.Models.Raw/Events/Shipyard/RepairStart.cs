namespace Sakuno.ING.Game.Events.Shipyard
{
    public class RepairStart
    {
        public RepairStart(bool instantRepair, int shipId, int repairingDockId)
        {
            InstantRepair = instantRepair;
            ShipId = shipId;
            RepairingDockId = repairingDockId;
        }

        public bool InstantRepair { get; }
        public int ShipId { get; }
        public int RepairingDockId { get; }
    }
}
