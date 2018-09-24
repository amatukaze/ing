using System;

namespace Sakuno.ING.Browser.Desktop
{
    public interface IBrowser : IDisposable
    {
        string Address { get; }
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        bool CanRefresh { get; }

        void GoBack();
        void GoForward();
        void Refresh();
        void Navigate(string address);

        event Action BrowserExited;
    }
}
