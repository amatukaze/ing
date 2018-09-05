using System;
using System.Windows;
using System.Windows.Interop;

namespace Sakuno.ING.Browser.Desktop
{
    public abstract class BrowserHost : HwndHost
    {
        public static readonly DependencyProperty AddressProperty
            = DependencyProperty.Register(nameof(Address), typeof(string), typeof(BrowserHost), new PropertyMetadata(string.Empty));
        public string Address
        {
            get => (string)GetValue(AddressProperty);
            set => SetValue(AddressProperty, value);
        }

        public static readonly DependencyProperty CanGoBackProperty
            = DependencyProperty.Register(nameof(CanGoBack), typeof(bool), typeof(BrowserHost), new PropertyMetadata(false));
        public bool CanGoBack
        {
            get => (bool)GetValue(CanGoBackProperty);
            set => SetValue(CanGoBackProperty, value);
        }

        public static readonly DependencyProperty CanGoForwardProperty
            = DependencyProperty.Register(nameof(CanGoForward), typeof(bool), typeof(BrowserHost), new PropertyMetadata(false));
        public bool CanGoForward
        {
            get => (bool)GetValue(CanGoForwardProperty);
            set => SetValue(CanGoForwardProperty, value);
        }

        public static readonly DependencyProperty CanRefreshProperty
            = DependencyProperty.Register(nameof(CanRefresh), typeof(bool), typeof(BrowserHost), new PropertyMetadata(false));
        public bool CanRefresh
        {
            get => (bool)GetValue(CanRefreshProperty);
            set => SetValue(CanRefreshProperty, value);
        }

        public abstract void GoBack();
        public abstract void GoForward();
        public abstract void Refresh();
        public abstract void Navigate(string address);

        public event Action BrowserExited;
        protected void NotifyBrowserExited() => BrowserExited?.Invoke();
    }
}
