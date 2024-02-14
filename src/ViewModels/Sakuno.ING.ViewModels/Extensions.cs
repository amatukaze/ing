using DynamicData;
using Sakuno.ING.Game;

namespace Sakuno.ING.ViewModels;

public static class Extensions
{
    public static IObservable<IChangeSet<T>> ToObservableChangeSet<T, TId>(this ITable<T, TId> source)
        where T : IIdentifiable<TId> =>
        source.ToObservableChangeSet<ITable<T, TId>, T>();
}
