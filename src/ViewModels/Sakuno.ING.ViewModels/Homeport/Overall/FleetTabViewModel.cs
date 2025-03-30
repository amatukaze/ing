using System.Reactive;

namespace Sakuno.ING.ViewModels.Homeport.Overall;

public class FleetTabViewModel : ReactiveObject
{
    public FleetId Id { get; }

    private readonly ObservableAsPropertyHelper<bool> _isSelected;
    public bool IsSelected => _isSelected.Value;

    public ReactiveCommand<Unit, Unit> SelectCommand { get; }

    public FleetTabViewModel(FleetId id, FleetSelectionState fleetSelectionState)
    {
        Id = id;
        SelectCommand = ReactiveCommand.Create(() => fleetSelectionState.Select(Id));

        _isSelected = fleetSelectionState.SelectedId.Select(selectedId => Id == selectedId).BindProperty(this, nameof(IsSelected));
    }
}
