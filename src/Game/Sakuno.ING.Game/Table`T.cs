using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace Sakuno.ING.Game;

public sealed class Table<T, TId, TRaw> : ITable<T, TId>, IDisposable
    where T : IModel<T, TId, TRaw>
    where TId : struct, IEquatable<TId>, IComparable<TId>
    where TRaw : IIdentifiable<TId>
{
    private readonly List<T> _list = new();

    private readonly IDisposable _fullUpdateSubscription;
    private readonly IDisposable? _partialUpdateSubscription;
    private readonly IDisposable? _removeSubscription;

    public int Count => _list.Count;

    public T? this[TId id]
    {
        get
        {
            var index = BinarySearch(id);

            return index >= 0 ? _list[index] : default;
        }
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public Table(IObservable<IReadOnlyList<TRaw>> fullUpdateSource, IObservable<IReadOnlyList<TRaw>>? partialUpdateSource = null, IObservable<IReadOnlyList<TId>>? removeSource = null)
    {
        _fullUpdateSubscription = fullUpdateSource.Subscribe(items =>
        {
            var i = 0;

            foreach (var item in items)
            {
                while (i < _list.Count && _list[i].Id.CompareTo(item.Id) < 0)
                    RemoveItem(i);

                if (i < _list.Count && _list[i].Id.Equals(item.Id))
                    _list[i++].Update(item);
                else
                    CreateItem(i++, item);
            }
        });
        _partialUpdateSubscription = partialUpdateSource?.Subscribe(items =>
        {
            foreach (var item in items)
            {
                var index = BinarySearch(item.Id);
                if (index >= 0)
                {
                    _list[index].Update(item);
                    continue;
                }

                CreateItem(~index, item);
            }
        });
        _removeSubscription = removeSource?.Subscribe(ids =>
        {
            foreach (var id in ids.OrderDescending())
            {
                var index = BinarySearch(id);
                if (index < 0)
                    continue;

                RemoveItem(index);
            }
        });
    }

    public List<T>.Enumerator GetEnumerator() => _list.GetEnumerator();

    public bool TryGetValue(TId id, [MaybeNullWhen(false)] out T value)
    {
        var index = BinarySearch(id);
        if (index < 0)
        {
            value = default;
            return false;
        }

        value = _list[index];
        return true;
    }

    private int BinarySearch(TId id)
    {
        var left = 0;
        var right = _list.Count - 1;

        while (left <= right)
        {
            var middle = left + ((right - left) >> 1);
            var result = _list[middle].Id.CompareTo(id);

            if (result is 0)
                return middle;

            if (result < 0)
                left = middle + 1;
            else
                right = middle - 1;
        }

        return ~left;
    }

    private void CreateItem(int index, TRaw raw)
    {
        var newItem = T.Create(raw);

        _list.Insert(index, newItem);
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Add, newItem, index));
    }
    private void RemoveItem(int index)
    {
        var item = _list[index];

        _list.RemoveAt(index);
        CollectionChanged?.Invoke(this, new(NotifyCollectionChangedAction.Remove, item, index));
    }

    public void Dispose()
    {
        _fullUpdateSubscription.Dispose();
        _partialUpdateSubscription?.Dispose();
        _removeSubscription?.Dispose();
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
}
