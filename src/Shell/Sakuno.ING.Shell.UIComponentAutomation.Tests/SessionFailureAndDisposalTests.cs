using Avalonia.Automation;
using static Sakuno.ING.Shell.UIComponentAutomation.Tests.SessionCoordinatorTests;

namespace Sakuno.ING.Shell.UIComponentAutomation.Tests;

public class SessionFailureAndDisposalTests
{
    [Fact]
    public async Task OpenConstructionFailurePreservesActiveAndDisposesCandidate()
    {
        var states = new List<TrackedScenarioState>();
        var candidates = new List<TrackedScenarioState>();
        var candidateResources = new List<ScopedResource>();
        var stable = TestScenarios.Counter("Tests/Counter/Stable", states);
        var failing = new ScenarioDefinition(
            "Tests/Counter/FailingOpen",
            "Control",
            registerServices: container => container.Register<ScopedResource>(Reuse.Scoped),
            createContent: scope =>
            {
                candidateResources.Add(scope.Resolve<ScopedResource>());
                var candidate = new TrackedScenarioState(candidates.Count + 1);
                candidates.Add(candidate);
                return TestScenarios.FailingContent(candidate);
            });
        using var coordinator = new UiComponentSessionCoordinator(new UiComponentScenarioCatalog(stable, failing));
        await coordinator.OpenAsync(stable.Id);
        await coordinator.ClickAsync("Increment");

        var exception = await AssertError(
            UiComponentAutomationErrorCodes.ConstructionFailure,
            () => coordinator.OpenAsync(failing.Id));

        Assert.Equal(failing.Id, exception.Error.Details["scenarioId"]);
        Assert.True(candidates.Single().IsDisposed);
        Assert.True(candidateResources.Single().IsDisposed);
        Assert.False(states[0].IsDisposed);
        var inspection = await coordinator.InspectAsync();
        Assert.Equal(stable.Id, inspection.ScenarioId);
        Assert.Equal(1, inspection.State["InstanceId"]);
        Assert.Equal(1, inspection.State["Count"]);
        Assert.NotEmpty((await coordinator.CaptureScreenshotAsync()).Png);
    }

    [Fact]
    public async Task OpenRegistrationFailureDisposesContainerAndPreservesActive()
    {
        var states = new List<TrackedScenarioState>();
        var stable = TestScenarios.Counter("Tests/Counter/StableRegistration", states);
        var resource = new ScopedResource();
        var failing = new ScenarioDefinition(
            "Tests/Counter/FailingRegistration",
            "Control",
            registerServices: container =>
            {
                container.RegisterInstance(resource);
                throw new InvalidOperationException("Test service registration failure.");
            },
            createContent: _ => throw new InvalidOperationException("Content must not be created after registration failure."));
        using var coordinator = new UiComponentSessionCoordinator(new UiComponentScenarioCatalog(stable, failing));
        await coordinator.OpenAsync(stable.Id);
        await coordinator.ClickAsync("Increment");

        var exception = await AssertError(
            UiComponentAutomationErrorCodes.ConstructionFailure,
            () => coordinator.OpenAsync(failing.Id));

        Assert.Equal(failing.Id, exception.Error.Details["scenarioId"]);
        Assert.Equal(1, resource.DisposeCount);
        Assert.False(states[0].IsDisposed);
        var inspection = await coordinator.InspectAsync();
        Assert.Equal(stable.Id, inspection.ScenarioId);
        Assert.Equal(1, inspection.State["Count"]);
        Assert.NotEmpty((await coordinator.CaptureScreenshotAsync()).Png);
    }

    [Fact]
    public async Task ResetConstructionFailurePreservesActiveAndDisposesCandidate()
    {
        var states = new List<TrackedScenarioState>();
        var definition = new ScenarioDefinition(
            "Tests/Counter/FailingReset",
            "StackPanel",
            createContent: _ =>
            {
                if (states.Count == 0)
                    return TestScenarios.CounterContent(states);

                var candidate = new TrackedScenarioState(2);
                states.Add(candidate);
                return TestScenarios.FailingContent(candidate);
            });
        using var coordinator = new UiComponentSessionCoordinator(new UiComponentScenarioCatalog(definition));
        await coordinator.OpenAsync(definition.Id);
        await coordinator.ClickAsync("Increment");

        await AssertError(UiComponentAutomationErrorCodes.ConstructionFailure, coordinator.ResetAsync);

        Assert.True(states[1].IsDisposed);
        Assert.False(states[0].IsDisposed);
        var inspection = await coordinator.InspectAsync();
        Assert.Equal(1, inspection.State["InstanceId"]);
        Assert.Equal(1, inspection.State["Count"]);
        Assert.NotEmpty((await coordinator.CaptureScreenshotAsync()).Png);
    }

    [Fact]
    public async Task CloseAndCoordinatorDisposeReleaseRootStateAndScope()
    {
        var states = new List<TrackedScenarioState>();
        var roots = new List<Control>();
        var resources = new List<ScopedResource>();
        var definition = new ScenarioDefinition(
            "Tests/Counter/Disposal",
            "StackPanel",
            registerServices: container => container.Register<ScopedResource>(Reuse.Scoped),
            createContent: scope =>
            {
                resources.Add(scope.Resolve<ScopedResource>());
                return TestScenarios.CounterContent(states, roots);
            });
        var coordinator = new UiComponentSessionCoordinator(new UiComponentScenarioCatalog(definition));
        await coordinator.OpenAsync(definition.Id);

        await coordinator.CloseAsync();

        Assert.True(states[0].IsDisposed);
        Assert.True(resources[0].IsDisposed);
        Assert.Null(UiComponentHost.Dispatch(() => roots[0].GetVisualParent()));
        await AssertError(UiComponentAutomationErrorCodes.NoActiveScenario, () => coordinator.InspectAsync());

        await coordinator.OpenAsync(definition.Id);
        coordinator.Dispose();

        Assert.True(states[1].IsDisposed);
        Assert.True(resources[1].IsDisposed);
        Assert.Null(UiComponentHost.Dispatch(() => roots[1].GetVisualParent()));
    }

    [Fact]
    public async Task RecoverableControlErrorsPreserveActiveForInspectionAndCapture()
    {
        var clicks = 0;
        var definition = new ScenarioDefinition(
            "Tests/Buttons/Errors",
            "StackPanel",
            createContent: _ =>
            {
                var root = new StackPanel();
                root.Children.Add(CreateButton("Duplicate", () => clicks++));
                root.Children.Add(CreateButton("Duplicate", () => clicks++));
                var disabled = CreateButton("Disabled", () => clicks++);
                disabled.IsEnabled = false;
                root.Children.Add(disabled);
                return new(root, () => new Dictionary<string, object?>());
            });
        using var coordinator = new UiComponentSessionCoordinator(new UiComponentScenarioCatalog(definition));
        await coordinator.OpenAsync(definition.Id);

        await AssertError(UiComponentAutomationErrorCodes.ControlNotFound, () => coordinator.ClickAsync("Missing"));
        await AssertError(UiComponentAutomationErrorCodes.AmbiguousControl, () => coordinator.ClickAsync("Duplicate"));
        await AssertError(UiComponentAutomationErrorCodes.ControlDisabled, () => coordinator.ClickAsync("Disabled"));
        await AssertError(UiComponentAutomationErrorCodes.ControlNotFound, () => coordinator.InspectAsync("Missing"));

        Assert.Equal(0, clicks);
        Assert.Equal(definition.Id, (await coordinator.InspectAsync()).ScenarioId);
        Assert.NotEmpty((await coordinator.CaptureScreenshotAsync()).Png);
    }

    [Fact]
    public async Task HostFailureIsStructuredAndDoesNotDiscardActiveSession()
    {
        var failInspection = false;
        var definition = new ScenarioDefinition(
            "Tests/Host/Failure",
            "Border",
            createContent: _ => new ScenarioContent(
                new Border(),
                () => failInspection
                    ? throw new InvalidOperationException("Test inspection failure.")
                    : new Dictionary<string, object?>()));
        using var coordinator = new UiComponentSessionCoordinator(new UiComponentScenarioCatalog(definition));
        await coordinator.OpenAsync(definition.Id);
        failInspection = true;

        var exception = await AssertError(UiComponentAutomationErrorCodes.HostFailure, () => coordinator.InspectAsync());

        Assert.Equal("inspect", exception.Error.Details["operation"]);
        failInspection = false;
        Assert.Equal(definition.Id, (await coordinator.InspectAsync()).ScenarioId);
        Assert.NotEmpty((await coordinator.CaptureScreenshotAsync()).Png);
    }

    private static Button CreateButton(string automationId, Action clicked)
    {
        var button = new Button { Content = automationId };
        AutomationProperties.SetAutomationId(button, automationId);
        button.Click += (_, _) => clicked();
        return button;
    }

    private sealed class ScopedResource : IDisposable
    {
        public int DisposeCount { get; private set; }
        public bool IsDisposed => DisposeCount > 0;

        public void Dispose() => DisposeCount++;
    }
}
