namespace Sakuno.ING.ViewModels;

public static class Extensions
{
    public static ObservableAsPropertyHelper<T> BindProperty<T, TObj>(this IObservable<T> observable, TObj source, string property, bool deferSubscription = false)
        where TObj : class, IReactiveObject =>
        observable.ToProperty(source, property, deferSubscription, scheduler: RxApp.MainThreadScheduler);
}
