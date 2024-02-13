using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events;

public interface IUnequippedSlotItemsUpdated
{
    SlotItemTypeId TypeId { get; }
    IReadOnlyList<SlotItemId> SlotItemIds { get; }
}
