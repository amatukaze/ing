using Sakuno.ING.Game.Provider;
using Sakuno.ING.ViewModels.Homeport.Overall;

namespace Sakuno.ING.Shell.UIComponentAutomation;

public sealed class UiComponentScenarioCatalog
{
    public const string FleetTabWithFleets = "Overall/FleetTab/WithFleets";

    public UiComponentScenarioCatalog()
    {
    }

    internal UiComponentScenarioCatalog(params ScenarioDefinition[] scenarios)
    {
        Scenarios = scenarios.ToArray();
    }

    public IReadOnlyList<ScenarioDefinition> Scenarios { get; } =
    [
        new(
            FleetTabWithFleets,
            "FleetTab",
            registerServices: container =>
            {
                container.RegisterInstance<IGameProvider>(new ScenarioGameProvider());
                container.Register<FleetSelectionState>(Reuse.Scoped);
            },
            createContent: scope =>
            {
                var selectionState = scope.Resolve<FleetSelectionState>();
                var state = new FleetScenarioState(selectionState);
                var viewModel = new FleetTabViewModel(FleetId.From(2), selectionState);
                var view = new Views.Overall.FleetTab { ViewModel = viewModel };

                return new(view, state.Capture, state);
            }),
    ];

    public UiComponentSession Open(string scenarioId)
    {
        var definition = Scenarios.SingleOrDefault(scenario => scenario.Id == scenarioId)
            ?? throw new KeyNotFoundException($"Unknown UI Component Scenario '{scenarioId}'.");

        return UiComponentSession.Open(definition);
    }
}
