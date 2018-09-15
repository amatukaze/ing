using System;
using System.Threading;
using System.Windows;
using Sakuno.CefSharp.Wpf;
using Sakuno.ING.Composition;
using Sakuno.ING.Http;
using CefClass = CefSharp.Cef;

namespace Sakuno.ING.Browser.Desktop.Cef
{
    [Export(typeof(IBrowserProvider))]
    sealed class CefBrowserProvider : IBrowserProvider
    {
        volatile int _isDisposed = 0;

        public string Id => "Cef";

        public IHttpProvider HttpProvider => throw new NotImplementedException();

        public void Initialize()
        {
            CefClass.Initialize(new DefaultCefSettings());
        }

        public BrowserHost CreateBrowser() => new CefWebView();
        public UIElement CreateSettingsView() => null;

        public void ClearCache() { }
        public void ClearCookie() => CefClass.GetGlobalCookieManager().DeleteCookies(null, null);

        void Dispose(bool disposing)
        {
            if (_isDisposed != 0 || Interlocked.CompareExchange(ref _isDisposed, 1, 0) != 0)
                return;

            CefClass.Shutdown();
        }

        ~CefBrowserProvider() => Dispose(false);
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
