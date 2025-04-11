namespace Sakuno.ING.ViewModels.Homeport;

public class RepairDockViewModel : ReactiveObject, IViewContractObservable
{
    public RepairDockId Id { get; }

    public IObservable<ShipInfoId> ShipMasterId { get; }
    public IObservable<DateTimeOffset> CompletionTime { get; }

    public IObservable<string?> ViewContractObservable { get; }

    public RepairDockViewModel(RepairDock model, PlayerDataService playerDataService)
    {
        Id = model.Id;

        ShipMasterId = model.WhenAnyValue(m => m.ShipId)
            .Select(id => playerDataService.Ships[id]?.MasterId ?? default)
            .ObserveOn(RxApp.MainThreadScheduler);
        CompletionTime = model.WhenAnyValue(m => m.CompletionTime).ObserveOn(RxApp.MainThreadScheduler);

        ViewContractObservable = model.WhenAnyValue(m => m.State).Select(s => s.ToString());
    }
}
