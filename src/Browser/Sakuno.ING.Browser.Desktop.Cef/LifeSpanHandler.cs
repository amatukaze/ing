using System;
using CefSharp;

namespace Sakuno.ING.Browser.Desktop.Cef
{
    using IBrowser = global::CefSharp.IBrowser;

    class LifeSpanHandler : ILifeSpanHandler
    {
        public bool DoClose(IWebBrowser chromiumWebBrowser, IBrowser browser) => false;
        public void OnAfterCreated(IWebBrowser chromiumWebBrowser, IBrowser browser) { }
        public void OnBeforeClose(IWebBrowser chromiumWebBrowser, IBrowser browser) { }

        public bool OnBeforePopup(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, string targetFrameName, WindowOpenDisposition targetDisposition, bool userGesture, IPopupFeatures popupFeatures, IWindowInfo windowInfo, IBrowserSettings browserSettings, ref bool noJavascriptAccess, out IWebBrowser newBrowser)
        {
            chromiumWebBrowser.Load(targetUrl);
            newBrowser = null;

            return true;
        }
    }
}
