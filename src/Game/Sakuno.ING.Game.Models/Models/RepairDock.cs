namespace Sakuno.ING.Game.Models
{
    public partial class RepairDock
    {
        partial void UpdateCore(RawRepairDock raw)
        {
            RepairingShip = _owner.Ships[raw.RepairingShipId];
        }
    }
}
