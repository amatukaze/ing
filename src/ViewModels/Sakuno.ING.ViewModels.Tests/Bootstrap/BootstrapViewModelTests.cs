using System.Reactive.Linq;
using System.Windows.Input;
using Microsoft.Extensions.Logging.Abstractions;
using Sakuno.ING.Bootstrap;
using Sakuno.ING.ViewModels.Bootstrap;

namespace Sakuno.ING.ViewModels.Tests.Bootstrap;

public class BootstrapViewModelTests
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

    private static BootstrapViewModel CreateViewModel(params IBootstrapStep[] steps) =>
        new(new BootstrapRunner(steps, NullLogger<BootstrapRunner>.Instance));

    [Fact]
    public async Task AllStepsSucceedCompletesCompletion()
    {
        var vm = CreateViewModel(new FakeBootstrapStep(200, "second"), new FakeBootstrapStep(100, "first"));

        vm.Start();

        await vm.Completion.WaitAsync(Timeout, TestContext.Current.CancellationToken);

        Assert.Equal(BootstrapStatus.Succeeded, vm.Status);
        Assert.False(vm.HasError);
        Assert.Equal(new[] { "first", "second" }, vm.Steps.Select(step => step.Name).ToArray());
        Assert.All(vm.Steps, step => Assert.Equal(BootstrapStepStatus.Succeeded, step.Status));
    }

    [Fact]
    public async Task FailedStepCanBeRetriedToSuccess()
    {
        var attempts = 0;
        var step = new FakeBootstrapStep(100, "flaky", _ => Task.FromResult(++attempts == 1 ? BootstrapStepResult.Failure("boom") : BootstrapStepResult.Success()));
        var vm = CreateViewModel(step);

        vm.Start();

        await WaitForFailureAsync(vm);
        Assert.True(((ICommand)vm.RetryCommand).CanExecute(null));
        Assert.False(((ICommand)vm.SkipCommand).CanExecute(null));

        vm.RetryCommand.Execute(Unit.Default).Subscribe();

        await vm.Completion.WaitAsync(Timeout, TestContext.Current.CancellationToken);

        Assert.Equal(2, step.RunCount);
        Assert.Equal(BootstrapStatus.Succeeded, vm.Status);
        Assert.False(vm.HasError);
    }

    [Fact]
    public async Task SkippableStepCanBeSkippedAndRunContinues()
    {
        var optional = new FakeBootstrapStep(100, "optional", _ => Task.FromResult(BootstrapStepResult.Failure("nope")), canSkip: true);
        var core = new FakeBootstrapStep(200, "core");
        var vm = CreateViewModel(optional, core);

        vm.Start();

        await WaitForFailureAsync(vm);
        Assert.True(((ICommand)vm.SkipCommand).CanExecute(null));

        vm.SkipCommand.Execute(Unit.Default).Subscribe();

        await vm.Completion.WaitAsync(Timeout, TestContext.Current.CancellationToken);

        Assert.Equal(BootstrapStepStatus.Skipped, vm.Steps[0].Status);
        Assert.Equal(BootstrapStepStatus.Succeeded, vm.Steps[1].Status);
        Assert.Equal(BootstrapStatus.Succeeded, vm.Status);
    }

    [Fact]
    public async Task NonSkippableStepDisablesSkipCommand()
    {
        var vm = CreateViewModel(new FakeBootstrapStep(100, "mandatory", _ => Task.FromResult(BootstrapStepResult.Failure("nope"))));

        vm.Start();

        await WaitForFailureAsync(vm);

        Assert.False(((ICommand)vm.SkipCommand).CanExecute(null));
        Assert.True(((ICommand)vm.RetryCommand).CanExecute(null));
    }

    [Fact]
    public async Task FailureMessageIsExposed()
    {
        var vm = CreateViewModel(new FakeBootstrapStep(100, "failing", _ => Task.FromResult(BootstrapStepResult.Failure("something broke"))));

        vm.Start();

        await WaitForFailureAsync(vm);

        Assert.Equal("something broke", vm.ErrorMessage);
        Assert.Equal("failing", vm.FailedStep?.Name);
        Assert.Equal(BootstrapStepStatus.Failed, vm.Steps[0].Status);
    }

    [Fact]
    public async Task ExitCancelsCompletionAndRunningStep()
    {
        var gate = new TaskCompletionSource();
        var step = new FakeBootstrapStep(100, "hanging", async ct =>
        {
            await gate.Task.WaitAsync(ct);
            return BootstrapStepResult.Success();
        });
        var vm = CreateViewModel(step);

        vm.Start();
        await vm.WhenAnyValue(viewModel => viewModel.IsRunning).FirstAsync(isRunning => isRunning);

        vm.ExitCommand.Execute(Unit.Default).Subscribe();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => vm.Completion);
    }

    private static async Task WaitForFailureAsync(BootstrapViewModel viewModel)
    {
        await viewModel.WhenAnyValue(vm => vm.FailedStep).FirstAsync(failedStep => failedStep is not null);
        await viewModel.WhenAnyValue(vm => vm.IsRunning).FirstAsync(isRunning => !isRunning);
    }
}
