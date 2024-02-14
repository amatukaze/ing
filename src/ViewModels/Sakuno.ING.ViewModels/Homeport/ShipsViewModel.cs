using Sakuno.ING.Game.Services;

namespace Sakuno.ING.ViewModels.Homeport;

public class ShipsViewModel : ReactiveObject
{
    private readonly ObservableAsPropertyHelper<int> _count;
    public int Count => _count.Value;

    public ShipsViewModel(PlayerDataService playerDataService)
    {
        _count = playerDataService.Ships.ToObservableChangeSet()
            .Count()
            .ToProperty(this, nameof(Count), deferSubscription: true, scheduler: RxApp.MainThreadScheduler);
    }
}
