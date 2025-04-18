namespace Sakuno.ING.Game.Events;

public interface IAdmiralUpdated
{
    string Name { get; }

    int Experience { get; }

    int MaxShipCount { get; }
    int MaxSlotItemCount { get; }
}
