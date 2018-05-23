using System.Collections.Generic;

namespace Sakuno.ING.Game
{
    public interface ITable<TId, out T> : IReadOnlyCollection<T>, IUpdationSource
        where TId : struct
    {
        T this[TId id] { get; }
        T TryGetIfValid(TId id);
        T TryGetOrDummy(TId id);
        T TryGetOrDummyIfValid(TId id);
        IBindableCollection<T> DefaultView { get; }
    }
}
