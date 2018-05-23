using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events.Shipyard
{
    public class RepairStart
    {
        public RepairStart(bool instantRepair, ShipId shipId, int repairingDockId)
        {
            InstantRepair = instantRepair;
            ShipId = shipId;
            RepairingDockId = repairingDockId;
        }

        public bool InstantRepair { get; }
        public ShipId ShipId { get; }
        public int RepairingDockId { get; }
    }
}
