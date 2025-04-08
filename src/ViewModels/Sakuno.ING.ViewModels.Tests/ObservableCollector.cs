namespace Sakuno.ING.ViewModels.Tests;

public class ObservableCollector<T> : IObserver<T>
{
    private readonly List<T> _values = new();
    public IReadOnlyList<T> Values => _values;

    public T LatestValue { get; private set; } = default!;

    void IObserver<T>.OnNext(T value)
    {
        _values.Add(value);

        LatestValue = value;
    }

    void IObserver<T>.OnError(Exception error)
    {
    }

    void IObserver<T>.OnCompleted()
    {
    }
}
