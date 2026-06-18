using Sakuno.ING.Game;

namespace Sakuno.ING.ViewModels.Homeport;

[RegisterScoped(Registration = RegistrationStrategy.Self)]
public class ShipCountViewModel : ViewModelObject
{
    public IObservable<int> Count { get; }
    public IObservable<int> MaxCount { get; }

    public ShipCountViewModel(IPlayerDataService playerDataService)
    {
        Count = playerDataService.Ships.Connect()
            .Count()
            .StartWith(0)
            .ObserveOn(RxSchedulers.MainThreadScheduler);
        MaxCount = playerDataService.Admiral.Select(a => a.MaxShipCount)
            .StartWith(0)
            .ObserveOn(RxSchedulers.MainThreadScheduler);
    }
}
