namespace Sakuno.ING.ViewModels.Homeport.Overall;

public interface ISelectedFleetStateProvider
{
    IObservable<FleetId> SelectedFleetId { get; }
}
