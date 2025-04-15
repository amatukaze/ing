namespace Sakuno.ING.ViewModels.Homeport;

public class FleetViewModel : ViewModelObject, IViewContractObservable
{
    public FleetId Id { get; }

    public IObservable<ExpeditionId> ExpeditionId { get; }

    public IObservable<string?> ViewContractObservable { get; }

    public FleetViewModel(Fleet fleet)
    {
        Id = fleet.Id;

        var expeditionState = Id == (FleetId)1
            ? Observable.Return(FleetExpeditionState.None)
            : fleet.WhenAnyValue(f => f.ExpeditionState);

        ExpeditionId = Id == (FleetId)1
            ? Observable.Return<ExpeditionId>(default)
            : fleet.WhenAnyValue(f => f.ExpeditionId).ObserveOn(RxApp.MainThreadScheduler);

        ViewContractObservable = expeditionState
            .Select(state => (state switch
            {
                FleetExpeditionState.InMission or
                    FleetExpeditionState.Completed or
                    FleetExpeditionState.Recalled => FleetState.Expedition,
                _ => FleetState.Idle,
            }).ToString());
    }
}
