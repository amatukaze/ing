using DynamicData;
using ReactiveUI;
using Sakuno.ING.Game;
using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Sakuno.ING.ViewModels
{
    internal static class Extensions
    {
        public static IReadOnlyCollection<T> Bind<TId, T>(this ITable<TId, T> table, CompositeDisposable? disposables = null) where TId : struct
        {
            var subscription = table.DefaultViewSource.ObserveOn(RxApp.MainThreadScheduler).Bind(out var result).Subscribe();

            if (disposables is not null)
                subscription.DisposeWith(disposables);

            return result;
        }
        public static IReadOnlyCollection<T> Bind<T>(this IObservable<IChangeSet<T>> source, CompositeDisposable? disposables = null)
        {
            var subscription = source.ObserveOn(RxApp.MainThreadScheduler).Bind(out var result).Subscribe();

            if (disposables is not null)
                subscription.DisposeWith(disposables);

            return result;
        }
        public static IReadOnlyCollection<T> Bind<TId, T>(this IObservable<IChangeSet<T, TId>> source, CompositeDisposable? disposables = null) where TId : struct
        {
            var subscription = source.ObserveOn(RxApp.MainThreadScheduler).Bind(out var result).Subscribe();

            if (disposables is not null)
                subscription.DisposeWith(disposables);

            return result;
        }
    }
}
