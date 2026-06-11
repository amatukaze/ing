namespace Sakuno.ING.ViewModels.Homeport.Overall;

public class ShipViewModel : ViewModelObject
{
    public ShipId Id { get; }

    public IObservable<ShipInfoId> MasterId { get; }

    public IObservable<int> Level { get; }

    public ShipViewModel(ShipId id, PlayerDataService playerDataService)
    {
        Id = id;

        var model = playerDataService.Ships.Snapshot[id];
        var masterId = model.WhenAnyValue(s => s.MasterId);

        MasterId = masterId.ObserveOn(RxSchedulers.MainThreadScheduler);

        Level = model.WhenAnyValue(m => m.Level).ObserveOn(RxSchedulers.MainThreadScheduler);
    }
}
