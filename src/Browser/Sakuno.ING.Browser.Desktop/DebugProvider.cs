using System;
using System.Windows;
using Sakuno.ING.Composition;
using Sakuno.ING.Http;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Browser.Desktop
{
#if DEBUG
    [Export(typeof(IBrowserProvider))]
#endif
    internal class DebugProvider : IBrowserProvider,IBrowser
    {
        public DebugProvider(IShellContextService shell) => provider = new DebugHttpProvider(shell);

        private readonly DebugHttpProvider provider;

        public string Id => "Debug";
        public IHttpProvider HttpProvider => provider;
        public bool IsSupported => true;

        public void ClearCache() { }
        public void ClearCookie() { }
        public IBrowser CreateBrowser() => this;
        public UIElement CreateSettingsView() => null;
        public void Dispose() { }

        public string Address { get; set; }
        public bool CanGoBack { get; }
        public bool CanGoForward => true;
        public bool CanRefresh { get; }

        public event Action BrowserExited
        {
            add { }
            remove { }
        }

        public void GoBack() { }
        public void GoForward() => provider.Send();

        public void Initialize() { }

        public void Navigate(string address) { }
        public void Refresh() { }
    }
}
