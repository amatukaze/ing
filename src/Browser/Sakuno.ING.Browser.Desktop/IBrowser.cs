using System;

namespace Sakuno.ING.Browser.Desktop
{
    public interface IBrowser : IDisposable
    {
        string Address { get; }
        bool CanGoBack { get; }
        bool CanGoForward { get; }
        bool CanRefresh { get; }
        bool LockGame { get; set; }

        void GoBack();
        void GoForward();
        void Refresh();
        void Navigate(string address);
        void ScaleTo(double scale);

        event Action BrowserExited;
    }
}
