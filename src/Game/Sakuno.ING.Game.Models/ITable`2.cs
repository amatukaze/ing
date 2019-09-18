using System.Collections.Generic;

namespace Sakuno.ING.Game
{
    public interface ITable<TId, out T> : IBindable, IReadOnlyCollection<T>, IUpdationSource
        where TId : struct
    {
        T this[TId id] { get; }
        T this[TId? id] { get; }
        T TryGet(TId id);
        T[] this[IReadOnlyCollection<TId> ids] { get; }
        IBindableCollection<T> DefaultView { get; }
    }
}
