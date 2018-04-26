using System.Collections.Generic;

namespace Sakuno.ING.Game
{
    public interface ITable<out T> : IReadOnlyCollection<T>, IUpdationSource
    {
        T this[int id] { get; }
        T TryGetOrDummy(int id);
        IBindableCollection<T> DefaultView { get; }
    }
}
