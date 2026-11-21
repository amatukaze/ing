namespace Sakuno.ING.Shell.UIComponentAutomation;

public sealed class UiComponentSessionCoordinator : IDisposable
{
    private readonly UiComponentScenarioCatalog _catalog;
    private readonly object _queueGate = new();
    private Task _tail = Task.CompletedTask;
    private UiComponentSession? _activeSession;
    private ScenarioDefinition? _activeDefinition;
    private bool _disposed;

    public UiComponentSessionCoordinator()
        : this(new UiComponentScenarioCatalog())
    {
    }

    public UiComponentSessionCoordinator(UiComponentScenarioCatalog catalog)
    {
        _catalog = catalog;
    }

    public Task OpenAsync(string scenarioId) =>
        Enqueue(() => ExecuteLifecycle("open", () => Open(scenarioId)));

    public Task ResetAsync() =>
        Enqueue(() => ExecuteLifecycle("reset", () =>
        {
            var definition = RequireActive().Definition;
            Open(definition);
        }));

    public Task CloseAsync() =>
        Enqueue(() => ExecuteLifecycle("close", () =>
        {
            var session = RequireActive().Session;
            session.Dispose();
            _activeSession = null;
            _activeDefinition = null;
        }));

    public Task<InspectionResult> InspectAsync(string? rootIdentifier = null, int maxDepth = 4, int maxNodes = 100) =>
        Enqueue(() => ExecuteActive("inspect", session => session.Inspect(rootIdentifier, maxDepth, maxNodes)));

    public Task ClickAsync(string identifier) =>
        Enqueue(() => ExecuteActive("click", session =>
        {
            var result = session.Click(identifier);
            Exception? exception = result.Status switch
            {
                ControlInteractionStatus.Success => null,
                ControlInteractionStatus.NotFound => UiComponentAutomationErrors.ControlNotFound(session.ScenarioId, identifier),
                ControlInteractionStatus.Ambiguous => UiComponentAutomationErrors.AmbiguousControl(session.ScenarioId, identifier),
                ControlInteractionStatus.Inoperable => UiComponentAutomationErrors.InoperableControl(session.ScenarioId, identifier),
                _ => new InvalidOperationException($"Unknown control interaction status '{result.Status}'."),
            };

            if (exception is not null)
                throw exception;
        }));

    public Task<ScreenshotCapture> CaptureScreenshotAsync() =>
        Enqueue(() => ExecuteActive("capture", session => session.CaptureScreenshot()));

    private void Open(string scenarioId)
    {
        var definition = _catalog.Scenarios.SingleOrDefault(scenario => scenario.Id == scenarioId)
            ?? throw UiComponentAutomationErrors.ScenarioNotFound(scenarioId);
        Open(definition);
    }

    private void Open(ScenarioDefinition definition)
    {
        UiComponentSession candidate;
        try
        {
            candidate = UiComponentSession.Open(definition);
        }
        catch (UiComponentAutomationException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw UiComponentAutomationErrors.ConstructionFailure(definition.Id, exception);
        }

        var previous = _activeSession;
        _activeSession = candidate;
        _activeDefinition = definition;
        previous?.Dispose();
    }

    private void ExecuteLifecycle(string operation, Action action)
    {
        try
        {
            action();
        }
        catch (UiComponentAutomationException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw UiComponentAutomationErrors.HostFailure(operation, exception);
        }
    }

    private void ExecuteActive(string operation, Action<UiComponentSession> action) =>
        ExecuteActive<object?>(operation, session =>
        {
            action(session);
            return null;
        });

    private T ExecuteActive<T>(string operation, Func<UiComponentSession, T> action)
    {
        var session = RequireActive().Session;
        try
        {
            return action(session);
        }
        catch (UiComponentAutomationException)
        {
            throw;
        }
        catch (Exception exception)
        {
            throw UiComponentAutomationErrors.HostFailure(operation, exception);
        }
    }

    private (UiComponentSession Session, ScenarioDefinition Definition) RequireActive() =>
        _activeSession is { } session && _activeDefinition is { } definition
            ? (session, definition)
            : throw UiComponentAutomationErrors.NoActiveScenario();

    private Task Enqueue(Action action)
    {
        lock (_queueGate)
        {
            ThrowIfDisposed();
            var task = _tail.ContinueWith(
                _ => action(),
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);
            _tail = ObserveCompletion(task);
            return task;
        }
    }

    private Task<T> Enqueue<T>(Func<T> action)
    {
        lock (_queueGate)
        {
            ThrowIfDisposed();
            var task = _tail.ContinueWith(
                _ => action(),
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);
            _tail = ObserveCompletion(task);
            return task;
        }
    }

    private static Task ObserveCompletion(Task task) =>
        task.ContinueWith(
            _ => { },
            CancellationToken.None,
            TaskContinuationOptions.None,
            TaskScheduler.Default);

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw UiComponentAutomationErrors.CoordinatorDisposed();
    }

    public void Dispose()
    {
        Task disposal;
        lock (_queueGate)
        {
            if (_disposed)
                return;

            _disposed = true;
            disposal = _tail.ContinueWith(
                _ =>
                {
                    _activeSession?.Dispose();
                    _activeSession = null;
                    _activeDefinition = null;
                },
                CancellationToken.None,
                TaskContinuationOptions.None,
                TaskScheduler.Default);
            _tail = ObserveCompletion(disposal);
        }

        disposal.GetAwaiter().GetResult();
        GC.SuppressFinalize(this);
    }
}
