using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Sakuno.ING;

public abstract class BindableObject : IBindable
{
    private readonly List<(SynchronizationContext? syncContext, PropertyChangedEventHandler handler)> _handlers = new();

    public event PropertyChangedEventHandler? PropertyChanged
    {
        add
        {
            if (value is null)
                return;

            lock (_handlers)
                _handlers.Add((SynchronizationContext.Current, value));
        }
        remove
        {
            if (value is null)
                return;

            lock (_handlers)
                for (var i = _handlers.Count - 1; i >= 0; i--)
                    if (_handlers[i].handler == value)
                        _handlers.RemoveAt(i);
        }
    }

    protected void NotifyPropertyChanged([CallerMemberName] string? propertyName = null) =>
        NotifyPropertyChanged(new PropertyChangedEventArgs(propertyName));

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected void NotifyPropertyChanged(PropertyChangedEventArgs args)
    {
        lock (_handlers)
            foreach (var (syncContext, handler) in _handlers)
                if (syncContext is not null)
                    syncContext.Post(o => handler(this, args), null);
                else
                    handler(this, args);
    }

    protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        NotifyPropertyChanged(propertyName);
        return true;
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected bool SetField<T>(ref T field, T value, PropertyChangedEventArgs args)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;

        field = value;
        NotifyPropertyChanged(args);
        return true;
    }
}
