using Sakuno.ING.Game.Models;

namespace Sakuno.ING.Game.Events;

public interface IShipUpdated
{
    ShipId Id { get; }
}
