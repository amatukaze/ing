using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    partial class Fleet
    {
        partial void UpdateCore(IRawFleet raw)
        {
            ships.Query = raw.ShipIds.Select(shipTable.TryGetOrDummy);
        }
    }
}
