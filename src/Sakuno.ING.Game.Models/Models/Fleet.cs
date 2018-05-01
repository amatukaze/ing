using System.Linq;

namespace Sakuno.ING.Game.Models
{
    partial class Fleet
    {
        partial void UpdateCore(IRawFleet raw)
        {
            ships.Query = raw.ShipIds.Select(shipTable.TryGetOrDummy);
            Expedition = expeditionInfoTable[raw.ExpeditionId];
        }
    }
}
