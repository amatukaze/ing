using System;
using System.Windows;
using Sakuno.ING.Http;

namespace Sakuno.ING.Browser.Desktop
{
    public interface IBrowserProvider : IDisposable
    {
        string Id { get; }
        IHttpProvider HttpProvider { get; }
        bool IsSupported { get; }

        IBrowser CreateBrowser();

        void ClearCache();
        void ClearCookie();
        UIElement CreateSettingsView();

        void Initialize();
    }
}
