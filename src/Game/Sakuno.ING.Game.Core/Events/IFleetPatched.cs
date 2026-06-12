using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events;

public interface IFleetPatched : IIdentifiable<FleetId>
{
    IReadOnlyList<ShipId> Ships { get; }
}
