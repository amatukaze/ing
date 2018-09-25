using System;
using System.Threading;
using System.Windows;
using Sakuno.CefSharp.Wpf;
using Sakuno.ING.Composition;
using Sakuno.ING.Http;
using CefClass = CefSharp.Cef;
using CefProxy = CefSharp.ProxyOptions;
using CefSetting = CefSharp.CefSharpSettings;

namespace Sakuno.ING.Browser.Desktop.Cef
{
    [Export(typeof(IBrowserProvider))]
    internal sealed class CefBrowserProvider : IBrowserProvider
    {
        private int _isDisposed = 0;

        public string Id => "Cef";

        private readonly IHttpProxy proxy;
        public IHttpProvider HttpProvider => proxy;

        public CefBrowserProvider(IHttpProxy proxy)
        {
            this.proxy = proxy;
        }

        public void Initialize()
        {
            CefSetting.Proxy = new CefProxy("localhost", proxy.ListeningPort.ToString());
            CefClass.Initialize(new DefaultCefSettings());

            proxy.IsEnabled = true;
        }

        public IBrowser CreateBrowser() => new CefHost();
        public UIElement CreateSettingsView() => null;

        public void ClearCache() { }
        public void ClearCookie() => CefClass.GetGlobalCookieManager().DeleteCookies(null, null);

        private void Dispose(bool disposing)
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
