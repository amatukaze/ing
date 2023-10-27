using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events.MasterData;

public interface IUseItemInfoUpdated
{
    UseItemId Id { get; }
    string Name { get; }
}
