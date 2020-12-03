using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events
{
    public sealed class RepairStart
    {
        public bool IsInstantRepair { get; }
        public ShipId ShipId { get; }
        public RepairDockId DockId { get; }

        public RepairStart(bool instantRepair, ShipId shipId, RepairDockId dockId)
        {
            IsInstantRepair = instantRepair;
            ShipId = shipId;
            DockId = dockId;
        }
    }
}
