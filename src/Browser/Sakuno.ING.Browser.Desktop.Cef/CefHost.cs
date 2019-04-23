using System;
using System.Windows;
using System.Windows.Media;
using Sakuno.CefSharp.Wpf;

namespace Sakuno.ING.Browser.Desktop.Cef
{
    internal class CefHost : CefWebBrowser, IBrowser
    {
        private double scale = 1;

        bool IBrowser.CanRefresh => IsBrowserInitialized;

        event Action IBrowser.BrowserExited
        {
            add { }
            remove { }
        }

        void IBrowser.Navigate(string address) => Address = address;
        public void ScaleTo(double scale)
        {
            this.scale = scale;
            UpdateScale();
        }

        private void UpdateScale()
        {
            if (!IsBrowserInitialized)
                return;

            GetBrowser().GetHost().SetZoomLevel(Math.Log(scale, 1.2));
        }

        public CefHost()
        {
            DragHandler = new DragHandler();
            LifeSpanHandler = new LifeSpanHandler();
            MenuHandler = new ContextMenuHandler();
            KeyboardHandler = new KeyboardHandler();

            FrameLoadStart += (s, e) => UpdateScale();
        }
    }
}
