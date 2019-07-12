using System;
using System.IO;
using System.Windows;
using Sakuno.CefSharp.Wpf;
using Sakuno.ING.Composition;
using Sakuno.ING.Http;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;
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
        private readonly IShellContextService shellContextService;

        public IHttpProvider HttpProvider => proxy;
        public bool IsSupported => true;

        private string cachePath;
        private readonly ISettingItem<bool> clearCacheOnStartup;

        public CefBrowserProvider(IHttpProxy proxy, IShellContextService shellContextService, ISettingsManager settings)
        {
            this.proxy = proxy;
            this.shellContextService = shellContextService;

            clearCacheOnStartup = settings.Register("cef.clear_cache_on_startup", false);
        }

        public void Initialize()
        {
            cachePath = Path.Combine(Environment.CurrentDirectory, "Browser Cache", "Cef");

            if (clearCacheOnStartup.Value)
            {
                try
                {
                    Directory.Delete(cachePath, true);

                    clearCacheOnStartup.Value = false;
                }
                catch
                {
                }
            }

            CefSetting.Proxy = new CefProxy("localhost", proxy.ListeningPort.ToString());
            CefSetting.SubprocessExitIfParentProcessClosed = true;
            CefClass.Initialize(new DefaultCefSettings
            {
                CachePath = cachePath
            });
            CefClass.GetGlobalCookieManager().SetStoragePath(Path.Combine(Environment.CurrentDirectory, "Browser Cookies", "Cef"), true);

            proxy.IsEnabled = true;
        }

        public IBrowser CreateBrowser() => new CefHost();
        public UIElement CreateSettingsView() => null;

        public void ClearCache()
        {
            clearCacheOnStartup.Value = true;

            shellContextService.Capture().ShowMessageAsync("Cache will be cleared on the next startup.", "Information");
        }
        public void ClearCookie() => CefClass.GetGlobalCookieManager().DeleteCookies(null, null);

        public void Dispose() => CefClass.Shutdown();
    }
}
