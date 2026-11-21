namespace Sakuno.ING.Shell.UIComponentAutomation.Tests;

public class SessionCoordinatorTests
{
    [Fact]
    public async Task OperationsWithoutActiveScenarioReturnStructuredErrors()
    {
        using var coordinator = new UiComponentSessionCoordinator();

        await AssertError(UiComponentAutomationErrorCodes.NoActiveScenario, () => coordinator.InspectAsync());
        await AssertError(UiComponentAutomationErrorCodes.NoActiveScenario, () => coordinator.ClickAsync("FleetTab_2"));
        await AssertError(UiComponentAutomationErrorCodes.NoActiveScenario, () => coordinator.CaptureScreenshotAsync());
        await AssertError(UiComponentAutomationErrorCodes.NoActiveScenario, coordinator.ResetAsync);
        await AssertError(UiComponentAutomationErrorCodes.NoActiveScenario, coordinator.CloseAsync);
    }

    [Fact]
    public async Task ConcurrentOperationsRunInRequestOrderOnTheUiThread()
    {
        var states = new List<TrackedScenarioState>();
        var uiThreadChecks = new List<bool>();
        var definition = TestScenarios.Counter("Tests/Counter/Concurrency", states, uiThreadChecks: uiThreadChecks);
        using var coordinator = new UiComponentSessionCoordinator(new UiComponentScenarioCatalog(definition));
        await coordinator.OpenAsync(definition.Id);

        var firstClick = coordinator.ClickAsync("Increment");
        var middleInspection = coordinator.InspectAsync();
        var capture = coordinator.CaptureScreenshotAsync();
        var secondClick = coordinator.ClickAsync("Increment");
        var finalInspection = coordinator.InspectAsync();
        await Task.WhenAll(firstClick, middleInspection, capture, secondClick, finalInspection);
        var middle = await middleInspection;
        var final = await finalInspection;
        var screenshot = await capture;

        Assert.Equal(1, middle.State["Count"]);
        Assert.Equal(2, final.State["Count"]);
        Assert.Equal(2, states.Single().Count);
        Assert.Equal(3, uiThreadChecks.Count);
        Assert.All(uiThreadChecks, Assert.True);
        Assert.NotEmpty(screenshot.Png);
    }

    [Fact]
    public async Task UnknownScenarioDoesNotReplaceActiveSession()
    {
        using var coordinator = new UiComponentSessionCoordinator();
        await coordinator.OpenAsync(UiComponentScenarioCatalog.FleetTabWithFleets);
        await coordinator.ClickAsync("FleetTab_2");

        var exception = await AssertError(
            UiComponentAutomationErrorCodes.ScenarioNotFound,
            () => coordinator.OpenAsync("Overall/FleetTab/Unknown"));

        Assert.Equal("Overall/FleetTab/Unknown", exception.Error.Details["scenarioId"]);
        var inspection = await coordinator.InspectAsync();
        Assert.Equal("2", inspection.State["SelectedFleetId"]);
        Assert.NotEmpty((await coordinator.CaptureScreenshotAsync()).Png);
    }

    [Fact]
    public async Task ReplacementDisposesPreviousSessionAndDoesNotShareMutableState()
    {
        var states = new List<TrackedScenarioState>();
        var roots = new List<Control>();
        var definition = TestScenarios.Counter("Tests/Counter/Replacement", states, roots);
        using var coordinator = new UiComponentSessionCoordinator(new UiComponentScenarioCatalog(definition));
        await coordinator.OpenAsync(definition.Id);
        await coordinator.ClickAsync("Increment");

        await coordinator.OpenAsync(definition.Id);

        Assert.Equal(2, states.Count);
        Assert.True(states[0].IsDisposed);
        Assert.False(states[1].IsDisposed);
        Assert.NotSame(roots[0], roots[1]);
        Assert.Null(UiComponentHost.Dispatch(() => roots[0].GetVisualParent()));
        Assert.Equal(0, (await coordinator.InspectAsync()).State["Count"]);
    }

    [Fact]
    public async Task ResetCreatesFreshUiAndPresentationState()
    {
        var states = new List<TrackedScenarioState>();
        var roots = new List<Control>();
        var definition = TestScenarios.Counter("Tests/Counter/Reset", states, roots);
        using var coordinator = new UiComponentSessionCoordinator(new UiComponentScenarioCatalog(definition));
        await coordinator.OpenAsync(definition.Id);
        await coordinator.ClickAsync("Increment");

        await coordinator.ResetAsync();

        var inspection = await coordinator.InspectAsync();
        Assert.Equal(2, inspection.State["InstanceId"]);
        Assert.Equal(0, inspection.State["Count"]);
        Assert.NotSame(roots[0], roots[1]);
        Assert.True(states[0].IsDisposed);
        Assert.False(states[1].IsDisposed);
        Assert.Null(UiComponentHost.Dispatch(() => roots[0].GetVisualParent()));
    }

    internal static async Task<UiComponentAutomationException> AssertError(string code, Func<Task> operation)
    {
        var exception = await Assert.ThrowsAsync<UiComponentAutomationException>(operation);
        Assert.Equal(code, exception.Error.Code);
        Assert.Equal(exception.Message, exception.Error.Message);
        Assert.False(string.IsNullOrWhiteSpace(exception.Error.Message));
        Assert.NotNull(exception.Error.Details);
        return exception;
    }
}
