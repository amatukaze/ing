using Microsoft.Extensions.Hosting;
using Sakuno.ING.Bootstrap;

namespace Sakuno.ING.Shell.Bootstrap;

[RegisterSingleton<IBootstrapStep>(Duplicate = DuplicateStrategy.Append)]
internal sealed class StartCoreServicesStep(IHost host) : IBootstrapStep
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);

    public int Order => 1000;
    public string Name => "启动核心服务";
    public bool CanSkip => false;

    public async Task<BootstrapStepResult> RunAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var timeoutCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeoutCts.CancelAfter(Timeout);

            await host.StartAsync(timeoutCts.Token);

            return BootstrapStepResult.Success();
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (OperationCanceledException)
        {
            return BootstrapStepResult.Failure($"核心服务启动超时({Timeout.TotalSeconds:N0} 秒)");
        }
        catch (Exception ex)
        {
            return BootstrapStepResult.Failure(ex.Message);
        }
    }
}
