using Microsoft.Extensions.Logging.Abstractions;
using Sakuno.ING.Bootstrap;
using Sakuno.ING.ViewModels.Bootstrap;

namespace Sakuno.ING.ViewModels.Tests.Bootstrap;

public class BootstrapRunnerTests
{
    private static BootstrapRunner CreateRunner(params IBootstrapStep[] steps) =>
        new(steps, NullLogger<BootstrapRunner>.Instance);

    [Fact]
    public async Task StepsRunInOrder()
    {
        var executed = new List<string>();
        var steps = new IBootstrapStep[]
        {
            new FakeBootstrapStep(200, "second", _ => { executed.Add("second"); return Task.FromResult(BootstrapStepResult.Success()); }),
            new FakeBootstrapStep(100, "first", _ => { executed.Add("first"); return Task.FromResult(BootstrapStepResult.Success()); }),
        };
        var progress = new List<BootstrapStepProgress>();

        var result = await CreateRunner(steps).RunAsync(progress.Add, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(new[] { "first", "second" }, executed);
        Assert.Equal(
            new[] { BootstrapStepStatus.Running, BootstrapStepStatus.Succeeded, BootstrapStepStatus.Running, BootstrapStepStatus.Succeeded },
            progress.Select(p => p.Status).ToArray());
    }

    [Fact]
    public async Task FailureStopsRunAndReportsFailedStep()
    {
        var first = new FakeBootstrapStep(100, "first");
        var second = new FakeBootstrapStep(200, "second", _ => Task.FromResult(BootstrapStepResult.Failure("boom")));
        var third = new FakeBootstrapStep(300, "third");

        var result = await CreateRunner(first, second, third).RunAsync(null, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Same(second, result.FailedStep);
        Assert.Equal("boom", result.ErrorMessage);
        Assert.Equal(1, second.RunCount);
        Assert.Equal(0, third.RunCount);
    }

    [Fact]
    public async Task SkippedStepDoesNotStopRun()
    {
        var second = new FakeBootstrapStep(200, "second");

        var result = await CreateRunner(
            new FakeBootstrapStep(100, "first", _ => Task.FromResult(BootstrapStepResult.Skipped())),
            second).RunAsync(null, CancellationToken.None);

        Assert.True(result.Succeeded);
        Assert.Equal(1, second.RunCount);
    }

    [Fact]
    public async Task StepExceptionIsReportedAsFailure()
    {
        var step = new FakeBootstrapStep(100, "exploding", _ => Task.FromException<BootstrapStepResult>(new InvalidOperationException("kaput")));

        var result = await CreateRunner(step).RunAsync(null, CancellationToken.None);

        Assert.False(result.Succeeded);
        Assert.Same(step, result.FailedStep);
        Assert.Equal("kaput", result.ErrorMessage);
    }

    [Fact]
    public async Task CancellationThrowsOperationCanceledException()
    {
        var step = new FakeBootstrapStep(100, "hanging", async ct =>
        {
            await Task.Delay(Timeout.InfiniteTimeSpan, ct);
            return BootstrapStepResult.Success();
        });

        using var cts = new CancellationTokenSource();
        var runTask = CreateRunner(step).RunAsync(null, cts.Token);
        await Task.Delay(50, TestContext.Current.CancellationToken);
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(() => runTask);
    }

    [Fact]
    public async Task RunFromIndexSkipsEarlierSteps()
    {
        var first = new FakeBootstrapStep(100, "first");
        var second = new FakeBootstrapStep(200, "second");

        var result = await CreateRunner(first, second).RunAsync(null, CancellationToken.None, startIndex: 1);

        Assert.True(result.Succeeded);
        Assert.Equal(0, first.RunCount);
        Assert.Equal(1, second.RunCount);
    }
}
