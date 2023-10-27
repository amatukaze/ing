using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.MasterData;

public interface IShipTypeInfoUpdated
{
    ShipTypeId Id { get; }
    string Name { get; }
}
