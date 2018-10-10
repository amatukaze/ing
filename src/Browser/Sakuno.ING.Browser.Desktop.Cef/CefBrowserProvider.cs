using System;
using System.IO;
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
        public string Id => "Cef";

        private readonly IHttpProxy proxy;
        public IHttpProvider HttpProvider => proxy;
        public bool IsSupported => true;

        public CefBrowserProvider(IHttpProxy proxy)
        {
            this.proxy = proxy;
        }

        public void Initialize()
        {
            CefSetting.Proxy = new CefProxy("localhost", proxy.ListeningPort.ToString());
            CefClass.Initialize(new DefaultCefSettings
            {
                CachePath = cachePath = Path.Combine(Environment.CurrentDirectory, "Cef", "Cache")
            });
            CefClass.GetGlobalCookieManager().SetStoragePath(Path.Combine(Environment.CurrentDirectory, "Cef", "Cookies"), true);

            proxy.IsEnabled = true;
        }

        public IBrowser CreateBrowser() => new CefHost();
        public UIElement CreateSettingsView() => null;

        private string cachePath;
        public void ClearCache()
        {
            try
            {
                Directory.Delete(cachePath, true);
            }
            catch { }
        }
        public void ClearCookie() => CefClass.GetGlobalCookieManager().DeleteCookies(null, null);

        public void Dispose() => CefClass.Shutdown();
    }
}
