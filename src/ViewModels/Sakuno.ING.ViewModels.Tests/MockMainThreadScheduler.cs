using System.Reactive.Concurrency;

namespace Sakuno.ING.ViewModels.Tests;

internal sealed class MockMainThreadScheduler : IScheduler
{
    public DateTimeOffset Now => DateTimeOffset.Now;

    public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action) => action(this, state);

    public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action) =>
        throw new NotImplementedException();
    public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action) =>
        throw new NotImplementedException();
}
