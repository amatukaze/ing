using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.MasterData;

public interface IMapAreaInfoUpdated
{
    MapAreaId Id { get; }
    string Name { get; }
}
