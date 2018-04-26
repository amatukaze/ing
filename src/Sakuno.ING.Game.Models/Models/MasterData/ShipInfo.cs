using System.Linq;

namespace Sakuno.ING.Game.Models.MasterData
{
    partial class ShipInfo
    {
        partial void UpdateCore(IRawShipInfo raw)
        {
            Type = shipTypeInfoTable[raw.TypeId];
            CanUpgrade = raw.UpgradeTo != 0;
            UpgradeTo = shipInfoTable.TryGetOrDummy(raw.UpgradeTo);
            TotalAircraft = Aircraft?.Sum();
        }
    }
}
