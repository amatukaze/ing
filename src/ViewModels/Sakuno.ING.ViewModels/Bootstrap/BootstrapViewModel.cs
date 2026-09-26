using Sakuno.ING.Bootstrap;

namespace Sakuno.ING.ViewModels.Bootstrap;

public enum BootstrapStatus
{
    Running,
    Failed,
    Succeeded,
}

[RegisterSingleton(Registration = RegistrationStrategy.Self)]
public class BootstrapViewModel : ViewModelObject
{
    private readonly BootstrapRunner _runner;
    private readonly Dictionary<IBootstrapStep, BootstrapStepViewModel> _stepMap;
    private readonly BootstrapStepViewModel[] _steps;
    private readonly CancellationTokenSource _cancellation = new();
    private readonly TaskCompletionSource _completion = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public IReadOnlyList<BootstrapStepViewModel> Steps => _steps;

    public Task Completion => _completion.Task;

    public BootstrapStatus Status
    {
        get;
        private set => SetField(ref field, value);
    } = BootstrapStatus.Running;

    public BootstrapStepViewModel? FailedStep
    {
        get;
        private set => SetField(ref field, value);
    }

    public string? ErrorMessage
    {
        get;
        private set => SetField(ref field, value);
    }

    public bool HasError
    {
        get;
        private set => SetField(ref field, value);
    }

    public bool IsRunning
    {
        get;
        private set => SetField(ref field, value);
    }

    public ReactiveCommand<Unit, Unit> RetryCommand { get; }
    public ReactiveCommand<Unit, Unit> SkipCommand { get; }
    public ReactiveCommand<Unit, Unit> ExitCommand { get; }

    public BootstrapViewModel(BootstrapRunner runner)
    {
        _runner = runner;

        _steps = _runner.Steps.Select(step => new BootstrapStepViewModel(step)).ToArray();
        _stepMap = _steps.ToDictionary(step => step.Step);

        var canRetry = this.WhenAnyValue(viewModel => viewModel.FailedStep)
            .CombineLatest(this.WhenAnyValue(viewModel => viewModel.IsRunning),
                (failedStep, isRunning) => failedStep is not null && !isRunning);
        var canSkip = this.WhenAnyValue(viewModel => viewModel.FailedStep)
            .CombineLatest(this.WhenAnyValue(viewModel => viewModel.IsRunning),
                (failedStep, isRunning) => failedStep is { CanSkip: true } && !isRunning);

        RetryCommand = ReactiveCommand.CreateFromTask(RetryAsync, canRetry);
        SkipCommand = ReactiveCommand.CreateFromTask(SkipAsync, canSkip);
        ExitCommand = ReactiveCommand.Create(Exit);

        Disposables.Add(_cancellation);
    }

    public void Start() => _ = RunAsync(0);

    private Task RetryAsync()
    {
        if (FailedStep is { } failedStep)
            return RunAsync(Array.IndexOf(_steps, failedStep));

        return Task.CompletedTask;
    }

    private Task SkipAsync()
    {
        if (FailedStep is not { } failedStep)
            return Task.CompletedTask;

        failedStep.Status = BootstrapStepStatus.Skipped;

        return RunAsync(Array.IndexOf(_steps, failedStep) + 1);
    }

    private void Exit()
    {
        _cancellation.Cancel();
        _completion.TrySetCanceled(_cancellation.Token);
    }

    private async Task RunAsync(int startIndex)
    {
        IsRunning = true;
        Status = BootstrapStatus.Running;
        FailedStep = null;
        ErrorMessage = null;
        HasError = false;

        try
        {
            var result = await _runner.RunAsync(OnProgress, _cancellation.Token, startIndex);

            if (result.Succeeded)
            {
                Status = BootstrapStatus.Succeeded;
                _completion.TrySetResult();
            }
            else if (result.FailedStep is { } failedStep && _stepMap.TryGetValue(failedStep, out var failedStepViewModel))
            {
                Status = BootstrapStatus.Failed;
                FailedStep = failedStepViewModel;
                ErrorMessage = result.ErrorMessage;
                HasError = true;
            }
        }
        catch (OperationCanceledException)
        {
            _completion.TrySetCanceled(_cancellation.Token);
        }
        catch (Exception ex)
        {
            Status = BootstrapStatus.Failed;
            ErrorMessage = ex.Message;
            HasError = true;
        }
        finally
        {
            IsRunning = false;
        }
    }

    private void OnProgress(BootstrapStepProgress progress)
    {
        if (!_stepMap.TryGetValue(progress.Step, out var step))
            return;

        step.Status = progress.Status;
        step.Message = progress.Message;
    }
}
