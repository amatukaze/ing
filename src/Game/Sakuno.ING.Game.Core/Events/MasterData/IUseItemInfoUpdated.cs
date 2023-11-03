using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.MasterData;

public interface IUseItemInfoUpdated : IIdentifiable<UseItemId>
{
    string Name { get; }
}
