using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events;

public interface IConstructionDockUpdated : IIdentifiable<ConstructionDockId>
{
    ConstructionDockState State { get; }
    Materials Consumption { get; }
    DateTimeOffset CompletionTime { get; }
    ShipInfoId ResultShipId { get; }
}
