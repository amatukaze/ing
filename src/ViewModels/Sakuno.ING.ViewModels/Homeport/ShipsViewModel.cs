namespace Sakuno.ING.ViewModels.Homeport;

public class ShipsViewModel : ViewModelObject
{
    public IObservable<int> Count { get; }

    public ShipsViewModel(PlayerDataService playerDataService)
    {
        Count = playerDataService.Ships.Connect().Count().ObserveOn(RxApp.MainThreadScheduler);
    }
}
