using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
#nullable disable
    internal sealed class HomeportJson
    {
        public RawShip[] api_ship { get; set; }
        public BasicAdmiral api_basic { get; set; }
        public RawRepairDock[] api_ndock { get; set; }
        public RawMaterialItem[] api_material { get; set; }
        public RawFleet[] api_deck_port { get; set; }
        public CombinedFleetKind api_combined_flag { get; set; }
    }
#nullable enable
}
