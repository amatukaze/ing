using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.MasterData;

public interface ISlotItemInfoUpdated
{
    SlotItemInfoId Id { get; }
    string Name { get; }

    SlotItemTypeId TypeId { get; }
    int IconId { get; }
    int PlaneId { get; }

    FireRange FireRange { get; }

    int Rarity { get; }
}
