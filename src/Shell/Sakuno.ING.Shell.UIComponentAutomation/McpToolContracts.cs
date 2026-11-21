namespace Sakuno.ING.Shell.UIComponentAutomation;

public sealed record ScenarioToolInfo(string Id, string ComponentName, int Width, int Height);
public sealed record ScenarioListResult(IReadOnlyList<ScenarioToolInfo> Scenarios);
public sealed record ScenarioOperationResult(string ScenarioId);
public sealed record OperationResult(string Status);
public sealed record ScreenshotMetadata(string ScenarioId, int Width, int Height);
