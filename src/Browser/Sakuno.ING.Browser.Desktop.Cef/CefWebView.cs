using Sakuno.CefSharp.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Sakuno.ING.Browser.Desktop.Cef
{
    class CefWebView : BrowserHost
    {
        CefWebBrowser _browser;

        public override void GoBack()
        {
            _browser.GoBack();
        }

        public override void GoForward()
        {
            _browser.GoForward();
        }

        public override void Navigate(string address)
        {
            _browser.Load(address);
        }

        public override void Refresh()
        {
            _browser.Refresh();
        }

        protected override HandleRef BuildWindowCore(HandleRef hwndParent)
        {
            _browser = new CefWebBrowser();

            return new HandleRef(null, _browser.Handle);
        }
        protected override void DestroyWindowCore(HandleRef hwnd) => _browser.Dispose();
    }
}
