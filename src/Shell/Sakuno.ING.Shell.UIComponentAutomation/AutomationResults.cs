namespace Sakuno.ING.Shell.UIComponentAutomation;

public sealed record ControlBounds(double X, double Y, double Width, double Height);

public sealed record ControlInspection(
    int Depth,
    string Type,
    string? AutomationId,
    string? Name,
    string? Text,
    bool IsVisible,
    bool IsEnabled,
    ControlBounds Bounds);

public sealed record InspectionResult(
    string ScenarioId,
    IReadOnlyList<ControlInspection> Controls,
    IReadOnlyDictionary<string, object?> State,
    bool IsTruncated);

public sealed record ScreenshotCapture(
    string ScenarioId,
    int Width,
    int Height,
    byte[] Png);

public enum ControlInteractionStatus
{
    Success,
    NotFound,
    Ambiguous,
    Disabled,
    Invisible,
    Inoperable,
}

public sealed record OperationDiagnostics(
    string Reason,
    string? ControlType = null,
    int? MatchCount = null,
    bool? IsVisible = null,
    bool? IsEnabled = null);

public sealed record ControlInteractionResult(
    ControlInteractionStatus Status,
    OperationDiagnostics? Diagnostics = null)
{
    public bool Succeeded => Status == ControlInteractionStatus.Success;
}
