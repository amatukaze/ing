using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events;

public interface IRepairDockUpdated
{
    RepairDockId Id { get; }
}
