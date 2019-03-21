using System;
using System.Windows;
using System.Windows.Media;
using Sakuno.CefSharp.Wpf;

namespace Sakuno.ING.Browser.Desktop.Cef
{
    internal class CefHost : CefWebBrowser, IBrowser
    {
        private double scale = 1;
        private DpiScale dpi;

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
            if (IsBrowserInitialized)
                GetBrowser().GetHost().SetZoomLevel((scale - 1) * dpi.DpiScaleX / 0.25);
        }

        public CefHost()
        {
            DragHandler = new DragHandler();
            LifeSpanHandler = new LifeSpanHandler();
            MenuHandler = new ContextMenuHandler();
            KeyboardHandler = new KeyboardHandler();

            dpi = VisualTreeHelper.GetDpi(this);
            FrameLoadStart += (s, e) => UpdateScale();
        }

        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            dpi = newDpi;
            UpdateScale();
        }
    }
}
