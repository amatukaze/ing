using ReactiveUI;
using Sakuno.ING.Composition;
using Splat;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Sakuno.ING.Shell.Desktop
{
    internal sealed class ViewModelViewHost : ContentControl, IViewFor, IEnableLogger, IDisposable
    {
        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null));

        public object? ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty DefaultContentProperty =
            DependencyProperty.Register(nameof(DefaultContent), typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null));

        public object? DefaultContent
        {
            get => GetValue(DefaultContentProperty);
            set => SetValue(DefaultContentProperty, value);
        }

        private readonly IDisposable _subscription;
        private bool _isDisposed;

        public ViewModelViewHost()
        {
            _subscription = this.WhenAnyValue(r => r.ViewModel).Select(vm =>
            {
                if (vm is not IViewContractObservable viewContractObservable)
                    return Observable.Return(new
                    {
                        ViewModel = vm,
                        Contract = (string?)null,
                    });

                return viewContractObservable.ViewContractObservable.DistinctUntilChanged().ObserveOn(RxApp.MainThreadScheduler).Select(contract => new
                {
                    ViewModel = (object?)vm,
                    Contract = contract,
                });
            }).Switch().Subscribe(r => ResolveViewForViewModel(r.ViewModel, r.Contract));
        }

        private void ResolveViewForViewModel(object? viewModel, string? contract)
        {
            if (viewModel is null)
            {
                Content = DefaultContent;
                return;
            }

            var viewLocator = Compositor.Default;
            var viewForType = typeof(IViewFor<>).MakeGenericType(viewModel.GetType());

            if ((viewLocator.ResolveOrDefault(viewForType, contract) ?? viewLocator.ResolveOrDefault(viewForType)) is not IViewFor viewInstance)
            {
                Content = DefaultContent;
                this.Log().Warn($"The {nameof(ViewModelViewHost)} could not find a valid view for the view model of type {viewModel.GetType()} and value {viewModel}.");
                return;
            }

            viewInstance.ViewModel = viewModel;

            Content = viewInstance;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _subscription.Dispose();

            _isDisposed = true;
        }
    }
}
