using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace Sakuno.ING.Views.Desktop.Controls
{
    public sealed class CountdownPresenter : Control
    {
        public static readonly DependencyProperty TimeProperty =
            DependencyProperty.Register(nameof(Time), typeof(DateTimeOffset?), typeof(CountdownPresenter), new PropertyMetadata(null, Update));

        public DateTimeOffset? Time
        {
            get => (DateTimeOffset?)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        private static readonly DispatcherTimer _timer;

        private readonly TextBlock _textBlock = new TextBlock();

        private bool _isAttached;

        static CountdownPresenter()
        {
            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1),
                IsEnabled = true,
            };
        }
        public CountdownPresenter()
        {
            AddVisualChild(_textBlock);

            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        protected override int VisualChildrenCount => 1;
        protected override Visual GetVisualChild(int index) => _textBlock;

        private void Attach()
        {
            if (_isAttached)
                return;

            _timer.Tick += OnTimerTick;
            _isAttached = true;
        }
        private void Detach()
        {
            if (!_isAttached)
                return;

            _timer.Tick -= OnTimerTick;
            _isAttached = false;
        }
        private void OnTimerTick(object? sender, EventArgs e) => UpdateCore();

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Attach();
            UpdateCore();
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Detach();
            UpdateCore();
        }

        private static void Update(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var i = (CountdownPresenter)d;
            var time = (DateTimeOffset?)e.NewValue;

            if (time.HasValue)
                i.Attach();
            else
                i.Detach();
        }

        private void UpdateCore()
        {
            if (Time is not DateTimeOffset time)
            {
                _textBlock.Text = "--:--:--";
                return;
            }

            var remainingTime = time - DateTimeOffset.Now;
            if (remainingTime <= TimeSpan.Zero)
                remainingTime = TimeSpan.Zero;

            _textBlock.Text = $"{(int)remainingTime.TotalHours:D2}:{remainingTime:mm\\:ss}";
        }
    }
}
