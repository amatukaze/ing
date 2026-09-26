namespace Sakuno.ING.Bootstrap;

public interface IBootstrapStep
{
    int Order { get; }
    string Name { get; }
    bool CanSkip { get; }

    Task<BootstrapStepResult> RunAsync(CancellationToken cancellationToken);
}
