using Sakuno.ING.Game.Models;
using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events;

public interface IFleetUpdated
{
    FleetId Id { get; }
    string Name { get; }
    IReadOnlyList<ShipId> Ships { get; }

    FleetExpeditionState ExpeditionState { get; }
    ExpeditionId ExpeditionId { get; }
    DateTimeOffset ExpeditionCompletionTime { get; }
}
