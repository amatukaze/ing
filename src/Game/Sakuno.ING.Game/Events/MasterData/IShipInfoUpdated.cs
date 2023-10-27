using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.MasterData;

public interface IShipInfoUpdated
{
    ShipInfoId Id { get; }
    string Name { get; }
}
