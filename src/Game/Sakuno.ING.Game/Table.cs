using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using DynamicData;
using DynamicData.Binding;

namespace Sakuno.ING.Game;

internal class Table<T, TId, TRaw> : ITable<T, TId>, IDisposable
    where T : IModel<T, TId, TRaw>
    where TId : struct, IEquatable<TId>, IComparable<TId>
    where TRaw : IIdentifiable<TId>
{
    protected readonly List<T> _list = [];

    private readonly IObservable<IChangeSet<T>> _changes;

    protected readonly CompositeDisposable _disposables = [];

    public int Count => _list.Count;

    public ITableSnapshot<T, TId> Snapshot => field ??= new SnapshotView(this);

    private readonly Subject<ITable<T, TId>>? _committed;

    public IObservable<ITable<T, TId>> Committed =>
        field ??=
            (_committed ?? throw new InvalidOperationException("Committed observable is not available"))
            .AsObservable();

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public Table(IObservable<IReadOnlyList<TRaw>> fullUpdateSource,
        IObservable<IReadOnlyList<TRaw>>? partialUpdateSource = null,
        IObservable<IReadOnlyList<TId>>? removeSource = null,
        IObservable<Unit>? committingSource = null)
        : this(fullUpdateSource, partialUpdateSource, removeSource, committingSource, false) { }
    protected Table(IObservable<IReadOnlyList<TRaw>> fullUpdateSource,
        IObservable<IReadOnlyList<TRaw>>? partialUpdateSource,
        IObservable<IReadOnlyList<TId>>? removeSource,
        IObservable<Unit>? committingSource,
        bool patchable)
    {
        _disposables.Add(fullUpdateSource.Subscribe(items =>
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
        }));

        if (patchable || partialUpdateSource is not null || removeSource is not null)
        {
            ArgumentNullException.ThrowIfNull(committingSource);

            _committed = new();
        }

        var partialUpdateSubscription = partialUpdateSource?.Buffer(committingSource!).Subscribe(events =>
        {
            foreach (var items in events)
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

            Commit();
        });

        if (partialUpdateSubscription is not null)
            _disposables.Add(partialUpdateSubscription);

        var removeSubscription = removeSource?.Buffer(committingSource!).Subscribe(events =>
        {
            foreach (var ids in events)
            foreach (var id in ids.OrderDescending())
            {
                var index = BinarySearch(id);
                if (index < 0)
                    continue;

                RemoveItem(index);
            }

            Commit();
        });

        if (removeSubscription is not null)
            _disposables.Add(removeSubscription);

        var changes = this.ToObservableChangeSet<ITable<T, TId>, T>().Publish();

        _changes = changes.AsObservable();
        _disposables.Add(changes.Connect());
    }

    protected void Commit() => _committed!.OnNext(this);

    public IObservable<IChangeSet<T>> Connect() => _changes;

    List<T>.Enumerator GetEnumerator() => _list.GetEnumerator();

    protected int BinarySearch(TId id)
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

    public void Dispose() => _disposables.Dispose();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();

    private sealed class SnapshotView(Table<T, TId, TRaw> owner) : ITableSnapshot<T, TId>
    {
        public int Count => owner.Count;

        public T this[TId id]
        {
            get
            {
                var index = owner.BinarySearch(id);
                if (index < 0)
                    throw new KeyNotFoundException($"{id} not found");

                return owner._list[index];
            }
        }

        public bool TryGetValue(TId id, [MaybeNullWhen(false)] out T value)
        {
            var index = owner.BinarySearch(id);
            if (index < 0)
            {
                value = default;
                return false;
            }

            value = owner._list[index];
            return true;
        }

        public IEnumerator<T> GetEnumerator() => owner._list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}

internal sealed class Table<T, TId, TRaw, TPatch> : Table<T, TId, TRaw>
    where T : IModel<T, TId, TRaw>, IPatchable<TId, TPatch>
    where TId : struct, IEquatable<TId>, IComparable<TId>
    where TRaw : IIdentifiable<TId>
    where TPatch : IPatch<TId>
{
    public Table(IObservable<IReadOnlyList<TRaw>> fullUpdateSource,
        IObservable<TPatch> patchSource,
        IObservable<Unit> committingSource,
        IObservable<IReadOnlyList<TRaw>>? partialUpdateSource = null,
        IObservable<IReadOnlyList<TId>>? removeSource = null)
        : base(fullUpdateSource, partialUpdateSource, removeSource, committingSource, true)
    {
        _disposables.Add(patchSource.Buffer(committingSource).Subscribe(events =>
        {
            foreach (var patch in events)
            {
                var index = BinarySearch(patch.Id);
                if (index < 0)
                    continue;

                _list[index].Patch(patch);
            }

            Commit();
        }));
    }
}
