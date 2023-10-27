using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events;

public interface IFleetUpdated
{
    FleetId Id { get; }
    string Name { get; }
    IReadOnlyList<ShipId> Ships { get; }
}
