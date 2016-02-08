using System.Collections.Generic;
using System.Linq;

namespace Sakuno.KanColle.Amatsukaze.Game.Models
{
    public static class ShipExtensions
    {
        public static IEnumerable<Ship> ExceptEvacuated(this IEnumerable<Ship> rpShips) => rpShips.Where(r => !r.State.HasFlag(ShipState.Evacuated));
    }
}
