using Avalonia.Automation;

namespace Sakuno.ING.Shell.UIComponentAutomation.Tests;

internal sealed class TrackedScenarioState : IDisposable
{
    private readonly int _instanceId;

    public TrackedScenarioState(int instanceId)
    {
        _instanceId = instanceId;
    }

    public int Count { get; set; }
    public bool IsDisposed { get; private set; }

    public IReadOnlyDictionary<string, object?> Capture() =>
        new Dictionary<string, object?>
        {
            ["InstanceId"] = _instanceId,
            ["Count"] = Count,
        };

    public void Dispose() => IsDisposed = true;
}

internal static class TestScenarios
{
    public static ScenarioDefinition Counter(
        string id,
        ICollection<TrackedScenarioState> states,
        ICollection<Control>? roots = null,
        ICollection<bool>? uiThreadChecks = null) =>
        new(
            id,
            "StackPanel",
            createContent: _ => CounterContent(states, roots, uiThreadChecks));

    public static ScenarioContent CounterContent(
        ICollection<TrackedScenarioState> states,
        ICollection<Control>? roots = null,
        ICollection<bool>? uiThreadChecks = null)
    {
        uiThreadChecks?.Add(Avalonia.Threading.Dispatcher.UIThread.CheckAccess());
        var state = new TrackedScenarioState(states.Count + 1);
        states.Add(state);
        var button = new Button { Content = "Increment" };
        AutomationProperties.SetAutomationId(button, "Increment");
        button.Click += (_, _) =>
        {
            state.Count++;
            uiThreadChecks?.Add(Avalonia.Threading.Dispatcher.UIThread.CheckAccess());
        };

        var root = new StackPanel { Children = { button } };
        roots?.Add(root);
        return new(root, state.Capture, state);
    }

    public static ScenarioContent FailingContent(TrackedScenarioState state) =>
        new(new ThrowOnInitializeControl(), state.Capture, state);

    private sealed class ThrowOnInitializeControl : Control
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();
            throw new InvalidOperationException("Test scenario construction failure.");
        }
    }
}
