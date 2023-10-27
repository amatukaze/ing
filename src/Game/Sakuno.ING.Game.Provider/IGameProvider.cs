using Sakuno.ING.Game.Events;
using Sakuno.ING.Game.Events.MasterData;

namespace Sakuno.ING.Game.Provider;

public interface IGameProvider
{
    IObservable<IMasterDataUpdated> MasterDataUpdated { get; }
}
