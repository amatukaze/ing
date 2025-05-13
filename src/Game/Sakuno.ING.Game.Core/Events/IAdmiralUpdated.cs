using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events;

public interface IAdmiralUpdated : IIdentifiable<AdmiralId>
{
    string Name { get; }

    int Experience { get; }

    int MaxShipCount { get; }
    int MaxSlotItemCount { get; }
}
