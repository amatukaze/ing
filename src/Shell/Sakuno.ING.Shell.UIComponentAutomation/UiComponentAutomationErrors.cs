namespace Sakuno.ING.Shell.UIComponentAutomation;

public static class UiComponentAutomationErrorCodes
{
    public const string ScenarioNotFound = "scenario_not_found";
    public const string NoActiveScenario = "no_active_scenario";
    public const string ConstructionFailure = "construction_failure";
    public const string ControlNotFound = "control_not_found";
    public const string AmbiguousControl = "ambiguous_control";
    public const string ControlDisabled = "control_disabled";
    public const string ControlInvisible = "control_invisible";
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
    private const int MaxReasonLength = 512;

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
                ["reason"] = BoundedReason(exception.Message),
            },
            exception);

    public static UiComponentAutomationException ControlNotFound(
        string scenarioId,
        string identifier,
        OperationDiagnostics? diagnostics = null) =>
        new(
            UiComponentAutomationErrorCodes.ControlNotFound,
            $"Control '{identifier}' was not found in UI Component Scenario '{scenarioId}'.",
            ControlDetails(scenarioId, identifier, diagnostics));

    public static UiComponentAutomationException AmbiguousControl(
        string scenarioId,
        string identifier,
        OperationDiagnostics? diagnostics = null) =>
        new(
            UiComponentAutomationErrorCodes.AmbiguousControl,
            $"Control identifier '{identifier}' is ambiguous in UI Component Scenario '{scenarioId}'.",
            ControlDetails(scenarioId, identifier, diagnostics));

    public static UiComponentAutomationException ControlDisabled(
        string scenarioId,
        string identifier,
        OperationDiagnostics? diagnostics = null) =>
        new(
            UiComponentAutomationErrorCodes.ControlDisabled,
            $"Control '{identifier}' is disabled in UI Component Scenario '{scenarioId}'.",
            ControlDetails(scenarioId, identifier, diagnostics));

    public static UiComponentAutomationException ControlInvisible(
        string scenarioId,
        string identifier,
        OperationDiagnostics? diagnostics = null) =>
        new(
            UiComponentAutomationErrorCodes.ControlInvisible,
            $"Control '{identifier}' is invisible in UI Component Scenario '{scenarioId}'.",
            ControlDetails(scenarioId, identifier, diagnostics));

    public static UiComponentAutomationException InoperableControl(
        string scenarioId,
        string identifier,
        OperationDiagnostics? diagnostics = null) =>
        new(
            UiComponentAutomationErrorCodes.InoperableControl,
            $"Control '{identifier}' cannot perform the requested operation in UI Component Scenario '{scenarioId}'.",
            ControlDetails(scenarioId, identifier, diagnostics));

    public static UiComponentAutomationException HostFailure(string operation, Exception exception) =>
        new(
            UiComponentAutomationErrorCodes.HostFailure,
            $"UI Component Automation host failed during '{operation}'.",
            new Dictionary<string, object?>
            {
                ["operation"] = operation,
                ["reason"] = BoundedReason(exception.Message),
            },
            exception);

    public static UiComponentAutomationException CoordinatorDisposed() =>
        new(
            UiComponentAutomationErrorCodes.HostFailure,
            "The UI Component Automation coordinator has been disposed.",
            new Dictionary<string, object?> { ["operation"] = "disposed" });

    private static string BoundedReason(string reason) =>
        reason.Length <= MaxReasonLength ? reason : reason[..MaxReasonLength];

    private static Dictionary<string, object?> ControlDetails(
        string scenarioId,
        string identifier,
        OperationDiagnostics? diagnostics = null)
    {
        var details = new Dictionary<string, object?>
        {
            ["scenarioId"] = scenarioId,
            ["identifier"] = identifier,
        };
        if (diagnostics is null)
            return details;

        details["reason"] = diagnostics.Reason;
        if (diagnostics.ControlType is not null)
            details["controlType"] = diagnostics.ControlType;
        if (diagnostics.MatchCount is not null)
            details["matchCount"] = diagnostics.MatchCount;
        if (diagnostics.IsVisible is not null)
            details["isVisible"] = diagnostics.IsVisible;
        if (diagnostics.IsEnabled is not null)
            details["isEnabled"] = diagnostics.IsEnabled;
        return details;
    }
}
