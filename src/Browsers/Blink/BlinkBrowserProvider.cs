using CefSharp;
using CefSharp.Wpf.HwndHost;
using Sakuno.KanColle.Amatsukaze.Extensibility.Browser;
using System;
using System.IO;
using System.Runtime;
using System.Threading.Tasks;
using IBrowser = Sakuno.KanColle.Amatsukaze.Extensibility.Browser.IBrowser;

namespace Sakuno.KanColle.Amatsukaze.Browser.Blink
{
    class BlinkBrowserProvider : IBrowserProvider
    {
        public string CoreDirectory { get; private set; }

        CefSettings r_Settings;

        BlinkBrowser r_Browser;
        public BlinkBrowser Browser => r_Browser;

        public Task Initialize(bool disableHWA, int port)
        {
            CoreDirectory = Path.GetDirectoryName(typeof(CefSettings).Assembly.Location);

            r_Settings = new CefSettings()
            {
                LogSeverity = LogSeverity.Fatal,
                BrowserSubprocessPath = Path.Combine(CoreDirectory, "CefSharp.BrowserSubprocess.exe"),
                CachePath = Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "Browser Cache", "Blink"),
                
            };

            var commandLines = r_Settings.CefCommandLineArgs;

            commandLines.Add("proxy-server", "localhost:" + port);
            commandLines.Add("disable-features", "HardwareMediaKeyHandling,MediaSessionService");

            if (disableHWA)
            {
                commandLines["disable-gpu"] = "1";
                commandLines["disable-gpu-compositing"] = "1";
            }

            CefSharpSettings.SubprocessExitIfParentProcessClosed = true;

            return Task.CompletedTask;
        }

        public Task Shutdown()
        {
            r_Browser.Dispose();
            Cef.Shutdown();

            return Task.CompletedTask;
        }

        public Task<IBrowser> CreateBrowserInstance()
        {
            if (!Cef.IsInitialized)
                Cef.Initialize(r_Settings, false, browserProcessHandler: null);

            return Task.FromResult<IBrowser>(r_Browser = new BlinkBrowser());
        }

        public async Task ClearCache()
        {
            if (r_Browser is null)
                return;

            using (var client = r_Browser.GetDevToolsClient())
                await client.Network.ClearBrowserCacheAsync();
        }

        public async Task ClearCookie()
        {
            if (r_Browser is null)
                return;

            using (var client = r_Browser.GetDevToolsClient())
                await client.Network.ClearBrowserCookiesAsync();
        }
    }
}
