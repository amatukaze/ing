using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;

namespace Sakuno.ING.Game;

public interface ITable<T, TId> : IReadOnlyCollection<T>, INotifyCollectionChanged
    where T : IIdentifiable<TId>
{
    T? this[TId id] { get; }

    bool TryGetValue(TId id, [MaybeNullWhen(false)] out T value);
}
