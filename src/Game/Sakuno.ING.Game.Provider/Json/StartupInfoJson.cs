using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Json
{
#nullable disable
    internal sealed class StartupInfoJson
    {
        public RawSlotItem[] api_slot_item { get; set; }
        public RawConstructionDock[] api_kdock { get; set; }
        public RawUseItemCount[] api_useitem { get; set; }
        public RawUnequippedSlotItemInfo[] api_unsetslot { get; set; }
    }
#nullable enable
}
