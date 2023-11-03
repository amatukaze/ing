using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.MasterData;

public interface IMapInfoUpdated : IIdentifiable<MapId>
{
    string Name { get; }
}
