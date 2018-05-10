using System.Collections.Generic;

namespace Sakuno.ING.Game
{
    public interface ITable<in TId, out T> : IReadOnlyCollection<T>, IUpdationSource
    {
        T this[TId id] { get; }
        T TryGetOrDummy(TId id);
        IBindableCollection<T> DefaultView { get; }
    }
}
