using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace Sakuno.ING.Game.Provider.Tests;

public class TestTable<T, TId> : ITableSnapshot<T, TId>
    where T : IIdentifiable<TId>
    where TId : notnull
{
    private readonly SortedList<TId, T> _list;

    public int Count => _list.Count;

    public T this[TId id] => _list[id];

    public TestTable(IEnumerable<T> list)
    {
        _list = new(list.ToDictionary(item => item.Id));
    }

    public bool TryGetValue(TId id, [MaybeNullWhen(false)] out T value) =>
        _list.TryGetValue(id, out value);

    public IEnumerator<T> GetEnumerator() => _list.Values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
