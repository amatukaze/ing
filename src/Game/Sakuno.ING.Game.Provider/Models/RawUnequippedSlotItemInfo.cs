using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Models
{
    public sealed class RawUnequippedSlotItemInfo
    {
        public SlotItemTypeId TypeId { get; set; }
        public SlotItemId[] SlotItemIds { get; set; } = default!;
    }
}
