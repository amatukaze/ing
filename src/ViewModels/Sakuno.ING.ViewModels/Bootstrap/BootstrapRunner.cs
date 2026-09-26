using Microsoft.Extensions.Logging;
using Sakuno.ING.Bootstrap;

namespace Sakuno.ING.ViewModels.Bootstrap;

public readonly record struct BootstrapStepProgress(IBootstrapStep Step, BootstrapStepStatus Status, string? Message = null);

public sealed record BootstrapRunResult(bool Succeeded, IBootstrapStep? FailedStep = null, string? ErrorMessage = null);

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public sealed class BootstrapRunner
{
    private readonly IBootstrapStep[] _steps;
    private readonly ILogger<BootstrapRunner> _logger;

    public IReadOnlyList<IBootstrapStep> Steps => _steps;

    public BootstrapRunner(IEnumerable<IBootstrapStep> steps, ILogger<BootstrapRunner> logger)
    {
        _steps = steps.OrderBy(step => step.Order).ToArray();
        _logger = logger;
    }

    public async Task<BootstrapRunResult> RunAsync(Action<BootstrapStepProgress>? progress, CancellationToken cancellationToken, int startIndex = 0)
    {
        for (var i = startIndex; i < _steps.Length; i++)
        {
            var step = _steps[i];

            cancellationToken.ThrowIfCancellationRequested();

            progress?.Invoke(new BootstrapStepProgress(step, BootstrapStepStatus.Running));

            try
            {
                var result = await step.RunAsync(cancellationToken);

                if (result.Status is BootstrapStepStatus.Failed)
                {
                    _logger.LogError("Bootstrap step '{StepName}' failed: {Message}", step.Name, result.Message);
                    progress?.Invoke(new BootstrapStepProgress(step, BootstrapStepStatus.Failed, result.Message));

                    return new BootstrapRunResult(Succeeded: false, FailedStep: step, ErrorMessage: result.Message);
                }

                progress?.Invoke(new BootstrapStepProgress(step, result.Status, result.Message));
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bootstrap step '{StepName}' threw an exception", step.Name);
                progress?.Invoke(new BootstrapStepProgress(step, BootstrapStepStatus.Failed, ex.Message));

                return new BootstrapRunResult(Succeeded: false, FailedStep: step, ErrorMessage: ex.Message);
            }
        }

        return new BootstrapRunResult(Succeeded: true);
    }
}
