namespace Sakuno.ING.Game.Models
{
    public partial class RepairDock
    {
        partial void UpdateCore(RawRepairDock raw)
        {
            RepairingShip = _owner.Ships[raw.RepairingShipId];
        }

        internal void InstantRepair()
        {
            State = RepairDockState.Empty;
            CompletionTime = null;
            RepairingShip = null;
        }
    }
}
