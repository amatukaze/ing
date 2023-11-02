using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.MasterData;

public interface ISlotItemTypeInfoUpdated
{
    SlotItemTypeId Id { get; }
    string Name { get; }
}
