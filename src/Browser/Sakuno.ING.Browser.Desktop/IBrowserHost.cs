using System;
using System.Windows;

namespace Sakuno.ING.Browser.Desktop
{
    public interface IBrowserHost : IBindable, IDisposable
    {
        UIElement BrowserElement { get; }
        string Address { get; }
        void Navigate(string address);

        bool CanGoBack { get; }
        void GoBack();
        bool CanGoForward { get; }
        void GoForward();
        bool CanRefresh { get; }
        void Refresh();
    }
}
