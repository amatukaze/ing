namespace Sakuno.ING.ViewModels.Homeport;

[RegisterScoped(Registration = RegistrationStrategy.Self)]
public class SlotItemCountViewModel : ViewModelObject
{
    public IObservable<int> Count { get; }
    public IObservable<int> MaxCount { get; }

    public SlotItemCountViewModel(PlayerDataService playerDataService)
    {
        Count = playerDataService.SlotItems.Connect()
            .Count()
            .StartWith(0)
            .ObserveOn(RxSchedulers.MainThreadScheduler);
        MaxCount = playerDataService.Admiral.Select(a => a.MaxSlotItemCount)
            .StartWith(0)
            .ObserveOn(RxSchedulers.MainThreadScheduler);
    }
}
