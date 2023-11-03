using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.MasterData;

public interface ISlotItemTypeInfoUpdated : IIdentifiable<SlotItemTypeId>
{
    string Name { get; }
}
