using System.Linq;

namespace Sakuno.ING.Game.Models.MasterData
{
    partial class ShipInfo
    {
        partial void UpdateCore(IRawShipInfo raw)
        {
            Type = owner.ShipTypes[raw.TypeId];
            CanUpgrade = raw.UpgradeTo != 0;
            UpgradeTo = owner.ShipInfos.TryGetOrDummy(raw.UpgradeTo);
            TotalAircraft = Aircraft?.Sum();
        }
    }
}
