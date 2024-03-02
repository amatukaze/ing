using CefSharp;

namespace Sakuno.KanColle.Amatsukaze.Browser.Blink.Handlers
{
    using IBlinkBrowser = global::CefSharp.IBrowser;

    class LifeSpanHandler : ILifeSpanHandler
    {
        public bool DoClose(IWebBrowser rpBrowserControl, IBlinkBrowser rpBrowser) => false;
        public void OnAfterCreated(IWebBrowser rpBrowserControl, IBlinkBrowser rpBrowser) { }
        public void OnBeforeClose(IWebBrowser rpBrowserControl, IBlinkBrowser rpBrowser) { }

        public bool OnBeforePopup(IWebBrowser rpBrowserControl, IBlinkBrowser rpBrowser, IFrame rpFrame, string rpTargetUrl, string rpTargetFrameName, WindowOpenDisposition rpTargetDisposition, bool rpUserGesture, IPopupFeatures rpPopupFeatures, IWindowInfo rpWindowInfo, IBrowserSettings rpBrowserSettings, ref bool rrpNoJavascriptAccess, out IWebBrowser ropNewBrowser)
        {
            rpBrowserControl.Load(rpTargetUrl);

            ropNewBrowser = rpBrowserControl;
            return true;
        }
    }
}
