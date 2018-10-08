using System;
using Sakuno.CefSharp.Wpf;

namespace Sakuno.ING.Browser.Desktop.Cef
{
    internal class CefHost : CefWebBrowser, IBrowser
    {
        bool IBrowser.CanRefresh => IsBrowserInitialized;

        event Action IBrowser.BrowserExited
        {
            add { }
            remove { }
        }

        void IBrowser.Navigate(string address) => Address = address;

        public CefHost()
        {
            DragHandler = new DragHandler();
            LifeSpanHandler = new LifeSpanHandler();
            MenuHandler = new ContextMenuHandler();
        }
    }
}
