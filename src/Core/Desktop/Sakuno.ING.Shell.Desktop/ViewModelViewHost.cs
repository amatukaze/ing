using ReactiveUI;
using Splat;
using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows;
using System.Windows.Controls;

namespace Sakuno.ING.Shell.Desktop
{
    internal class ViewModelViewHost : ContentControl, IViewFor, IEnableLogger, IDisposable
    {
        public static readonly DependencyProperty DefaultContentProperty =
            DependencyProperty.Register(nameof(DefaultContent), typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null));

        public object DefaultContent
        {
            get => GetValue(DefaultContentProperty);
            set => SetValue(DefaultContentProperty, value);
        }

        public static readonly DependencyProperty ViewModelProperty =
            DependencyProperty.Register(nameof(ViewModel), typeof(object), typeof(ViewModelViewHost), new PropertyMetadata(null, new PropertyChangedCallback(ViewModelChanged)));

        private static void ViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((ViewModelViewHost)d)._updateViewModel.OnNext(Unit.Default);

        public object? ViewModel
        {
            get => GetValue(ViewModelProperty);
            set => SetValue(ViewModelProperty, value);
        }

        public static readonly DependencyProperty ViewContractObservableProperty =
            DependencyProperty.Register(nameof(ViewContractObservable), typeof(IObservable<string>), typeof(ViewModelViewHost), new PropertyMetadata(Observable.Return<string?>(null), ViewContractChanged));

        private static void ViewContractChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) =>
            ((ViewModelViewHost)d)._updateViewContract.OnNext(Unit.Default);

        public IObservable<string?> ViewContractObservable
        {
            get => (IObservable<string?>)GetValue(ViewContractObservableProperty);
            set => SetValue(ViewContractObservableProperty, value);
        }

        private readonly Subject<Unit> _updateViewModel = new Subject<Unit>();
        private readonly Subject<Unit> _updateViewContract = new Subject<Unit>();

        private string? _viewContract;
        public string? ViewContract
        {
            get => _viewContract;
            set => ViewContractObservable = Observable.Return(value);
        }

        public IViewLocator? ViewLocator { get; set; }

        private bool _isDisposed;

        public ViewModelViewHost()
        {
            if (ModeDetector.InUnitTestRunner())
            {
                ViewContractObservable = Observable.Never<string>();
                return;
            }

            var contractChanged = _updateViewContract.Select(_ => ViewContractObservable).Switch();
            var viewModelChanged = _updateViewModel.Select(_ => ViewModel);

            contractChanged.CombineLatest(viewModelChanged, (contract, vm) => new
            {
                ViewModel = vm,
                Contract = contract
            }).Subscribe(x => ResolveViewForViewModel(x.ViewModel, x.Contract));

            contractChanged.ObserveOn(RxApp.MainThreadScheduler).Subscribe(x =>_viewContract = x ?? string.Empty);

            ViewContractObservable = Observable.FromEvent<SizeChangedEventHandler, string?>(
                eventHandler =>
                {
                    void Handler(object? sender, SizeChangedEventArgs e) => eventHandler(null!);
                    return Handler;
                },
                x => SizeChanged += x,
                x => SizeChanged -= x)
                .StartWith((string?)null)
                .DistinctUntilChanged();
        }

        public void Dispose()
        {
            Dispose(isDisposing: true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed)
                return;

            if (isDisposing)
            {
                _updateViewModel.Dispose();
                _updateViewContract.Dispose();
            }

            _isDisposed = true;
        }

        private void ResolveViewForViewModel(object? viewModel, string? contract)
        {
            if (viewModel is null)
            {
                Content = DefaultContent;
                return;
            }

            var viewLocator = ViewLocator ?? ReactiveUI.ViewLocator.Current;
            var viewInstance = viewLocator.ResolveView(viewModel, contract) ?? viewLocator.ResolveView(viewModel);

            if (viewInstance is null)
            {
                Content = DefaultContent;
                this.Log().Warn($"The {nameof(ViewModelViewHost)} could not find a valid view for the view model of type {viewModel.GetType()} and value {viewModel}.");
                return;
            }

            viewInstance.ViewModel = viewModel;

            Content = viewInstance;
        }
    }
}
