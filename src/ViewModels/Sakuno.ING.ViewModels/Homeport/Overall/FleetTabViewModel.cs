using System.Reactive;

namespace Sakuno.ING.ViewModels.Homeport.Overall;

public class FleetTabViewModel : ReactiveObject
{
    public FleetId Id { get; }

    private readonly ObservableAsPropertyHelper<bool> _isSelected;
    public bool IsSelected => _isSelected.Value;

    public ReactiveCommand<Unit, Unit> SelectCommand { get; } = ReactiveCommand.Create(() => { });

    public FleetTabViewModel(FleetId id, ISelectedFleetStateProvider selectedFleetStateProvider)
    {
        Id = id;

        _isSelected = selectedFleetStateProvider.SelectedFleetId.Select(selectedId => Id == selectedId).BindProperty(this, nameof(IsSelected));
    }
}
