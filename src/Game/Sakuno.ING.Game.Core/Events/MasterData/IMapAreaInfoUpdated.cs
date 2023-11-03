using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.MasterData;

public interface IMapAreaInfoUpdated : IIdentifiable<MapAreaId>
{
    string Name { get; }
}
