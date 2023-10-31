using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events;

public interface ISlotItemUpdated
{
    SlotItemId Id { get; }
    SlotItemInfoId MasterId { get; }

    bool IsLocked { get; }

    int ImprovementLevel { get; }
    int AerialProficiency { get; }
}
