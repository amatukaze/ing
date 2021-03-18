using Sakuno.ING.Game.Models;
using System.Collections.Generic;

namespace Sakuno.ING.Game.Events
{
    public sealed record SlotItemImproved(SlotItemId SlotItemId, bool IsSuccessful, RawSlotItem NewRawData, IReadOnlyCollection<SlotItemId> ConsumedSlotItemIds)
    {
    }
}
