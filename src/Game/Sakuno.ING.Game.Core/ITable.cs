using System.Collections.Specialized;
using DynamicData;

namespace Sakuno.ING.Game;

public interface ITable<T, TId> : IReadOnlyCollection<T>, INotifyCollectionChanged
    where T : IIdentifiable<TId>
{
    ITableSnapshot<T, TId> Snapshot { get; }

    IObservable<IChangeSet<T>> Connect();
}
