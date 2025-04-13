namespace Sakuno.ING.ViewModels.Homeport.Overall;

public class ShipViewModel : ViewModelObject
{
    public ShipId Id { get; }

    public IObservable<ShipInfoId> MasterId { get; }

    public ShipViewModel(ShipId id, PlayerDataService playerDataService)
    {
        Id = id;

        var model = playerDataService.Ships[id];
        var masterId = model.WhenAnyValue(s => s.MasterId);

        MasterId = masterId.ObserveOn(RxApp.MainThreadScheduler);
    }
}
