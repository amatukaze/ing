using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.MasterData;

public interface IMapInfoUpdated
{
    MapId Id { get; }
    string Name { get; }
}
