using DynamicData;
using Sakuno.ING.Game;

namespace Sakuno.ING.ViewModels;

public static class Extensions
{
    public static ObservableAsPropertyHelper<T> BindProperty<T, TObj>(this IObservable<T> observable, TObj source, string property)
        where TObj : class, IReactiveObject =>
        observable.ToProperty(source, property, deferSubscription: true, scheduler: RxApp.MainThreadScheduler);

    public static IObservable<IChangeSet<T>> ToObservableChangeSet<T, TId>(this ITable<T, TId> source)
        where T : IIdentifiable<TId> =>
        source.ToObservableChangeSet<ITable<T, TId>, T>();
}
