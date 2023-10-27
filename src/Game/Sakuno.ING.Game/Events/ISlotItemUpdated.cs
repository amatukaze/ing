using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events;

public interface ISlotItemUpdated
{
    SlotItemId Id { get; }
}
