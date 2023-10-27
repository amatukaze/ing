using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.MasterData;

public interface ISlotItemInfoUpdated
{
    SlotItemInfoId Id { get; }
    string Name { get; }
}
