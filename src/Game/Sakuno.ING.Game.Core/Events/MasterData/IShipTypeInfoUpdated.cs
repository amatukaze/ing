using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.MasterData;

public interface IShipTypeInfoUpdated : IIdentifiable<ShipTypeId>
{
    string Name { get; }
}
