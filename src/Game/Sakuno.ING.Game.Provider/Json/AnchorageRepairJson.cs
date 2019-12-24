using System.Collections.Generic;
using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
    internal class AnchorageRepairJson
    {
        public ShipId api_used_ship;
        public IReadOnlyList<ShipId> api_repair_ships;

        public RawShip[] api_ship_data;
    }
}
