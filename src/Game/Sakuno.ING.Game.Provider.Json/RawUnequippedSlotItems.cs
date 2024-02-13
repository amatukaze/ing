using Sakuno.ING.Game.Events;

namespace Sakuno.ING.Game.Provider.Json;

public sealed class RawUnequippedSlotItems : IUnequippedSlotItemsUpdated
{
    public SlotItemTypeId TypeId { get; set; }
    public SlotItemId[] SlotItemIds { get; set; } = default!;

    IReadOnlyList<SlotItemId> IUnequippedSlotItemsUpdated.SlotItemIds => SlotItemIds;
}
