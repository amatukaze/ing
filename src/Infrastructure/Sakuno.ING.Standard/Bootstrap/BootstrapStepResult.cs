namespace Sakuno.ING.Bootstrap;

public readonly record struct BootstrapStepResult(BootstrapStepStatus Status, string? Message = null)
{
    public static BootstrapStepResult Success() => new(BootstrapStepStatus.Succeeded);

    public static BootstrapStepResult Failure(string message) => new(BootstrapStepStatus.Failed, message);

    public static BootstrapStepResult Skipped(string? message = null) => new(BootstrapStepStatus.Skipped, message);
}
