using System;
using System.Windows;

namespace Sakuno.ING.Browser.Desktop
{
    public interface IBrowserProvider : IDisposable
    {
        string Id { get; }

        BrowserHost CreateBrowser();

        void ClearCache();
        void ClearCookie();
        UIElement CreateSettingsView();

        void Initialize();
    }
}
