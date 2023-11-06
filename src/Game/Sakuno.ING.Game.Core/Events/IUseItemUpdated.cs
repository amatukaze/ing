using Sakuno.ING.Game.Models.MasterData;

namespace Sakuno.ING.Game.Events;

public interface IUseItemUpdated : IIdentifiable<UseItemId>
{
    int Amount { get; }
}
