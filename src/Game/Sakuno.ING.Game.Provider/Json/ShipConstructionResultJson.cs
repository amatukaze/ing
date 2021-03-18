using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
#nullable disable
    internal sealed class ShipConstructionResultJson
    {
        public RawConstructionDock[] api_kdock { get; set; }
        public RawShip api_ship { get; set; }
        public RawSlotItem[] api_slotitem { get; set; }
    }
#nullable enable
}
