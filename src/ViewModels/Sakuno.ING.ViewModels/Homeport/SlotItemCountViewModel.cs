using Sakuno.ING.Game;

namespace Sakuno.ING.ViewModels.Homeport;

[RegisterScoped(Registration = RegistrationStrategy.Self)]
public class SlotItemCountViewModel : ViewModelObject
{
    private readonly HashSet<SlotItemInfoId> _excludedMasterIds =
    [
        SlotItemInfoId.From(42),
        SlotItemInfoId.From(43),
        SlotItemInfoId.From(145),
        SlotItemInfoId.From(146),
        SlotItemInfoId.From(150),
        SlotItemInfoId.From(241),
    ];

    public IObservable<int> Count { get; }
    public IObservable<int> MaxCount { get; }

    public SlotItemCountViewModel(IPlayerDataService playerDataService)
    {
        Count = playerDataService.SlotItems.Connect()
            .Filter(item => !_excludedMasterIds.Contains(item.MasterId))
            .Count()
            .StartWith(0)
            .ObserveOn(RxSchedulers.MainThreadScheduler);
        MaxCount = playerDataService.Admiral.Select(a => a.MaxSlotItemCount)
            .StartWith(0)
            .ObserveOn(RxSchedulers.MainThreadScheduler);
    }
}
