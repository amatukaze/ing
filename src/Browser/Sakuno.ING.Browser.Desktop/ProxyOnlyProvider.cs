using System.Windows;
using Sakuno.ING.Composition;
using Sakuno.ING.Http;

namespace Sakuno.ING.Browser.Desktop
{
    [Export(typeof(IBrowserProvider))]
    class ProxyOnlyProvider : IBrowserProvider
    {
        public string Id => "ProxyOnly";

        IHttpProxy _httpProxy;
        public IHttpProvider HttpProvider => _httpProxy;

        public ProxyOnlyProvider(IHttpProxy httpProxy)
        {
            _httpProxy = httpProxy;
        }

        public void Initialize() => _httpProxy.IsEnabled = true;

        public BrowserHost CreateBrowser() => null;
        public UIElement CreateSettingsView() => null;

        public void ClearCache() { }
        public void ClearCookie() { }

        public void Dispose() { }
    }
}
