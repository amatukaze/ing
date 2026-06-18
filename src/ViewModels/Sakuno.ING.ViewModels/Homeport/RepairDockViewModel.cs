using Sakuno.ING.Game;

namespace Sakuno.ING.ViewModels.Homeport;

public class RepairDockViewModel : ViewModelObject, IViewContractObservable
{
    public RepairDockId Id { get; }

    public IObservable<ShipInfoId> ShipMasterId { get; }
    public IObservable<DateTimeOffset> CompletionTime { get; }

    public IObservable<string?> ViewContractObservable { get; }

    public RepairDockViewModel(RepairDock model, IPlayerDataService playerDataService)
    {
        Id = model.Id;

        ShipMasterId = model.WhenAnyValue(m => m.ShipId)
            .Select(id => playerDataService.Ships.Snapshot.TryGetValue(id, out var ship) ? ship.MasterId : default)
            .ObserveOn(RxSchedulers.MainThreadScheduler);
        CompletionTime = model.WhenAnyValue(m => m.CompletionTime).ObserveOn(RxSchedulers.MainThreadScheduler);

        ViewContractObservable = model.WhenAnyValue(m => m.State).Select(s => s.ToString());
    }
}
