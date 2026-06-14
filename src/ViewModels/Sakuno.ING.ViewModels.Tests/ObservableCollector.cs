namespace Sakuno.ING.ViewModels.Tests;

public sealed class ObservableCollector<T> : IObserver<T>
{
    private readonly List<T> _values = new();
    public IReadOnlyList<T> Values => _values;

    public T? LatestValue => _values.Count > 0 ? _values[^1] : default;
    public Exception? LatestError { get; private set; }
    public bool IsCompleted { get; private set; }

    void IObserver<T>.OnNext(T value) => _values.Add(value);
    void IObserver<T>.OnError(Exception error) => LatestError = error;
    void IObserver<T>.OnCompleted() => IsCompleted = true;
}
