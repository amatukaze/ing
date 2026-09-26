using Sakuno.ING.Bootstrap;

namespace Sakuno.ING.ViewModels.Tests.Bootstrap;

internal sealed class FakeBootstrapStep : IBootstrapStep
{
    private readonly Func<CancellationToken, Task<BootstrapStepResult>> _run;

    public int Order { get; }
    public string Name { get; }
    public bool CanSkip { get; }
    public int RunCount { get; private set; }

    public FakeBootstrapStep(int order, string name, Func<CancellationToken, Task<BootstrapStepResult>>? run = null, bool canSkip = false)
    {
        Order = order;
        Name = name;
        CanSkip = canSkip;
        _run = run ?? (_ => Task.FromResult(BootstrapStepResult.Success()));
    }

    public Task<BootstrapStepResult> RunAsync(CancellationToken cancellationToken)
    {
        RunCount++;
        return _run(cancellationToken);
    }
}
