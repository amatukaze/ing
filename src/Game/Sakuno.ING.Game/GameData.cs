using System.Reactive.Subjects;

namespace Sakuno.ING.Game;

public sealed class GameData<T> : IObservable<T>, IDisposable
{
    private readonly ReplaySubject<T> _subject = new(1);

    private readonly IDisposable _subscription;

    public GameData(IObservable<T> source)
    {
        _subscription = source.Subscribe(_subject);
    }

    public IDisposable Subscribe(IObserver<T> observer) => _subject.Subscribe(observer);

    public void Dispose()
    {
        _subscription.Dispose();
        _subject.Dispose();
    }
}
