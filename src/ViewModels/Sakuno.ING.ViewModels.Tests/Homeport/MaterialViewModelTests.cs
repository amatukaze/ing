using Microsoft.Reactive.Testing;
using ReactiveUI;

namespace Sakuno.ING.ViewModels.Tests.Homeport;

public class MaterialViewModelTests : ReactiveTest
{
    private readonly TestScheduler _scheduler = new();

    public MaterialViewModelTests()
    {
        RxSchedulers.TaskpoolScheduler = _scheduler;
    }

    [Fact]
    public void CurrentReturnsLatestValue()
    {
        var source = _scheduler.CreateColdObservable(
            OnNext(TimeSpan.FromSeconds(1L).Ticks, 100),
            OnNext(TimeSpan.FromSeconds(2L).Ticks, 80)
        );
        var vm = new MaterialViewModel(source);
        var current = _scheduler.CreateObserver<int>();
        vm.Current.Subscribe(current);

        _scheduler.Start();

        Assert.Equal(2, current.Messages.Count);
        Assert.Equal(100, current.Messages[0].Value.Value);
        Assert.Equal(80, current.Messages[1].Value.Value);
    }

    [Fact]
    public void DifferenceComputedBetweenUpdates()
    {
        var source = _scheduler.CreateColdObservable(
            OnNext(TimeSpan.FromSeconds(1L).Ticks, 100),
            OnNext(TimeSpan.FromSeconds(5L).Ticks, 150),
            OnNext(TimeSpan.FromSeconds(6L).Ticks, 180),
            OnNext(TimeSpan.FromSeconds(7L).Ticks, 225)
        );
        var vm = new MaterialViewModel(source);
        var difference = _scheduler.CreateObserver<int>();
        vm.Difference.Subscribe(difference);

        _scheduler.Start();

        Assert.Equal(2, difference.Messages.Count);
        Assert.Equal(100, difference.Messages[0].Value.Value);
        Assert.Equal(125, difference.Messages[1].Value.Value);
    }
}
