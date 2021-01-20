using DynamicData;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Sakuno.ING.Game
{
    public interface ITable<TId, T> : IBindable, IReadOnlyCollection<T>
        where TId : struct
    {
        T this[TId id] { get; }
        [MaybeNull]
        T this[TId? id] { get; }
        T[] this[IReadOnlyCollection<TId> ids] { get; }

        IObservable<IChangeSet<T, TId>> DefaultViewSource { get; }

        [return: MaybeNull]
        T GetValueOrDefault(TId id);
    }
}
