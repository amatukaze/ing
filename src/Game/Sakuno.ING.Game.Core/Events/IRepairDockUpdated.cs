using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events;

public interface IRepairDockUpdated : IIdentifiable<RepairDockId>
{
    RepairDockState State { get; }
    ShipId ShipId { get; }
    DateTimeOffset CompletionTime { get; }
    Materials Consumption { get; }
}
