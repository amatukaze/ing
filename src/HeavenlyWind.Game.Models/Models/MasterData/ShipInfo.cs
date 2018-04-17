using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models.MasterData
{
    partial class ShipInfo
    {
        partial void UpdateCore(IRawShipInfo raw)
        {
            Type = shipTypeInfos[raw.TypeId];
            CanUpgrade = raw.UpgradeTo != 0;
            UpgradeTo = shipInfos.TryGetOrDummy(raw.UpgradeTo);
            TotalAircraft = Aircraft.Sum();
        }
    }
}
