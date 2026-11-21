using System.Reactive.Linq;
using Sakuno.ING.ViewModels.Homeport.Overall;

namespace Sakuno.ING.Shell.UIComponentAutomation;

internal sealed class FleetScenarioState : IDisposable
{
    private readonly IDisposable _subscription;
    private FleetId _selectedFleetId;

    public FleetScenarioState(FleetSelectionState selectionState)
    {
        _subscription = selectionState.SelectedId.Subscribe(id => _selectedFleetId = id);
    }

    public IReadOnlyDictionary<string, object?> Capture() =>
        new Dictionary<string, object?>
        {
            ["SelectedFleetId"] = _selectedFleetId.ToString(),
        };

    public void Dispose() => _subscription.Dispose();
}
