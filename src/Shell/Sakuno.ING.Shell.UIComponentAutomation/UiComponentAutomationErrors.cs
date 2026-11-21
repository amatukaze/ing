namespace Sakuno.ING.Shell.UIComponentAutomation;

public static class UiComponentAutomationErrorCodes
{
    public const string ScenarioNotFound = "scenario_not_found";
    public const string NoActiveScenario = "no_active_scenario";
    public const string ConstructionFailure = "construction_failure";
    public const string ControlNotFound = "control_not_found";
    public const string AmbiguousControl = "ambiguous_control";
    public const string InoperableControl = "inoperable_control";
    public const string HostFailure = "host_failure";
}

public sealed record UiComponentAutomationError(
    string Code,
    string Message,
    IReadOnlyDictionary<string, object?> Details);

public sealed class UiComponentAutomationException : Exception
{
    public UiComponentAutomationError Error { get; }

    internal UiComponentAutomationException(
        string code,
        string message,
        IReadOnlyDictionary<string, object?>? details = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Error = new(code, message, details ?? new Dictionary<string, object?>());
    }
}

internal static class UiComponentAutomationErrors
{
    public static UiComponentAutomationException ScenarioNotFound(string scenarioId) =>
        new(
            UiComponentAutomationErrorCodes.ScenarioNotFound,
            $"UI Component Scenario '{scenarioId}' is not registered.",
            new Dictionary<string, object?> { ["scenarioId"] = scenarioId });

    public static UiComponentAutomationException NoActiveScenario() =>
        new(
            UiComponentAutomationErrorCodes.NoActiveScenario,
            "No UI Component Scenario is active.");

    public static UiComponentAutomationException ConstructionFailure(string scenarioId, Exception exception) =>
        new(
            UiComponentAutomationErrorCodes.ConstructionFailure,
            $"Failed to construct UI Component Scenario '{scenarioId}'.",
            new Dictionary<string, object?>
            {
                ["scenarioId"] = scenarioId,
                ["reason"] = exception.Message,
            },
            exception);

    public static UiComponentAutomationException ControlNotFound(string scenarioId, string identifier) =>
        new(
            UiComponentAutomationErrorCodes.ControlNotFound,
            $"Control '{identifier}' was not found in UI Component Scenario '{scenarioId}'.",
            ControlDetails(scenarioId, identifier));

    public static UiComponentAutomationException AmbiguousControl(string scenarioId, string identifier) =>
        new(
            UiComponentAutomationErrorCodes.AmbiguousControl,
            $"Control identifier '{identifier}' is ambiguous in UI Component Scenario '{scenarioId}'.",
            ControlDetails(scenarioId, identifier));

    public static UiComponentAutomationException InoperableControl(string scenarioId, string identifier) =>
        new(
            UiComponentAutomationErrorCodes.InoperableControl,
            $"Control '{identifier}' is not visible and enabled in UI Component Scenario '{scenarioId}'.",
            ControlDetails(scenarioId, identifier));

    public static UiComponentAutomationException HostFailure(string operation, Exception exception) =>
        new(
            UiComponentAutomationErrorCodes.HostFailure,
            $"UI Component Automation host failed during '{operation}'.",
            new Dictionary<string, object?>
            {
                ["operation"] = operation,
                ["reason"] = exception.Message,
            },
            exception);

    public static UiComponentAutomationException CoordinatorDisposed() =>
        new(
            UiComponentAutomationErrorCodes.HostFailure,
            "The UI Component Automation coordinator has been disposed.",
            new Dictionary<string, object?> { ["operation"] = "disposed" });

    private static Dictionary<string, object?> ControlDetails(string scenarioId, string identifier) =>
        new()
        {
            ["scenarioId"] = scenarioId,
            ["identifier"] = identifier,
        };
}
