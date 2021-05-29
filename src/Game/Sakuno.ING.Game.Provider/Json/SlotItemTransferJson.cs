using Sakuno.ING.Game.Models;
using System.Collections.Generic;

namespace Sakuno.ING.Game.Json
{
#nullable disable
    internal sealed class SlotItemTransferJson
    {
        public ShipData api_ship_data { get; set; }

        public int api_bauxite { get; set; }

        public IEnumerable<RawShip> Ships
        {
            get
            {
                yield return api_ship_data.api_set_ship;
                yield return api_ship_data.api_unset_ship;
            }
        }

        public class ShipData
        {
            public RawShip api_set_ship { get; set; }
            public RawShip api_unset_ship { get; set; }
        }
    }
#nullable enable
}
