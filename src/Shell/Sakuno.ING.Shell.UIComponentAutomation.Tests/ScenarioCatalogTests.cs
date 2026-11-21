namespace Sakuno.ING.Shell.UIComponentAutomation.Tests;

public class ScenarioCatalogTests
{
    private readonly UiComponentScenarioCatalog _catalog = new();

    [Fact]
    public void CatalogMetadataIsUnique()
    {
        Assert.NotEmpty(_catalog.Scenarios);
        Assert.Equal(_catalog.Scenarios.Count, _catalog.Scenarios.Select(scenario => scenario.Id).Distinct().Count());
        Assert.Equal(_catalog.Scenarios.Count, _catalog.Scenarios.Select(scenario => scenario.ComponentName).Distinct().Count());
        Assert.Equal("FleetTab", _catalog.Scenarios.Single(scenario =>
            scenario.Id == UiComponentScenarioCatalog.FleetTabWithFleets).ComponentName);
    }

    [Fact]
    public void UnknownScenarioIsRejected()
    {
        Assert.Throws<KeyNotFoundException>(() => _catalog.Open("Overall/FleetTab/Missing"));
    }

    [Fact]
    public void FleetScenarioUsesOwnedScopesAndInMemoryGameProvider()
    {
        using var first = _catalog.Open(UiComponentScenarioCatalog.FleetTabWithFleets);
        using var second = _catalog.Open(UiComponentScenarioCatalog.FleetTabWithFleets);

        var firstProvider = Assert.IsType<ScenarioGameProvider>(first.Scope.Resolve<IGameProvider>());
        var secondState = second.Scope.Resolve<FleetSelectionState>();

        Assert.NotSame(first.Scope.Resolve<FleetSelectionState>(), secondState);
        Assert.Equal([FleetId.From(1), FleetId.From(2), FleetId.From(3)], firstProvider.PublishedFleetIds);
        Assert.Equal("1", first.Inspect().State["SelectedFleetId"]);
        Assert.Equal(FleetId.From(2), UiComponentHost.Dispatch(() =>
            Assert.IsType<FleetTabViewModel>(first.Root.DataContext).Id));
        Assert.IsType<AutomationApplication>(UiComponentHost.Dispatch(() => Application.Current));
        Assert.DoesNotContain(AppDomain.CurrentDomain.GetAssemblies(), assembly =>
            assembly.GetName().Name == "Sakuno.ING.Shell");
    }
}
