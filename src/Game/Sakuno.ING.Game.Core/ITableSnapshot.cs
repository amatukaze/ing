using System.Diagnostics.CodeAnalysis;

namespace Sakuno.ING.Game;

public interface ITableSnapshot<T, TId> : IReadOnlyCollection<T>
    where T : IIdentifiable<TId>
{
    T this[TId id] { get; }

    bool TryGetValue(TId id, [MaybeNullWhen(false)] out T value);
}
