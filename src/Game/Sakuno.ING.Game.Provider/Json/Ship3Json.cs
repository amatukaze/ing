using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
#nullable disable
    internal sealed class Ship3Json
    {
        public RawShip[] api_ship_data { get; set; }
        public RawFleet[] api_deck_data { get; set; }
        public RawUnequippedSlotItemInfo[] api_slot_data { get; set; }
    }
#nullable enable
}
