using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events;

public interface IFleetUpdated : IIdentifiable<FleetId>
{
    string Name { get; }
    IReadOnlyList<ShipId> Ships { get; }

    FleetExpeditionState ExpeditionState { get; }
    ExpeditionId ExpeditionId { get; }
    DateTimeOffset ExpeditionCompletionTime { get; }
}
