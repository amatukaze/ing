namespace Sakuno.ING.ViewModels.Homeport;

public class ConstructionDockViewModel : ViewModelObject, IViewContractObservable
{
    public ConstructionDockId Id { get; }

    public IObservable<ShipInfoId> ShipId { get; }
    public IObservable<DateTimeOffset> CompletionTime { get; }

    public IObservable<string?> ViewContractObservable { get; }

    public ConstructionDockViewModel(ConstructionDock model)
    {
        Id = model.Id;

        ShipId = model.WhenAnyValue(m => m.ResultShipId).ObserveOn(RxSchedulers.MainThreadScheduler);
        CompletionTime = model.WhenAnyValue(m => m.CompletionTime).ObserveOn(RxSchedulers.MainThreadScheduler);

        ViewContractObservable = model.WhenAnyValue(m => m.State).Select(s => s.ToString());
    }
}
