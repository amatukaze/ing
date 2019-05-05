using System;
using Sakuno.CefSharp.Wpf;
using Sakuno.ING.Settings;

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
            if (IsBrowserInitialized)
                GetBrowser().GetHost().SetZoomLevel(Math.Log(scale, 1.2));
        }

        public CefHost()
        {
            DragHandler = new DragHandler();
            LifeSpanHandler = new LifeSpanHandler();
            MenuHandler = new ContextMenuHandler();
            KeyboardHandler = new KeyboardHandler();

            FrameLoadStart += (s, e) =>
            {
                UpdateScale();
                if (LockGame && CanExecuteJavascriptInMainFrame)
                    GetBrowser().MainFrame.ExecuteJavaScriptAsync(BrowserSetting.StyleSheetSetJs);
            };
        }

        private bool _lockGame;
        public bool LockGame
        {
            get => _lockGame;
            set
            {
                if (_lockGame != value)
                {
                    _lockGame = value;
                    if (CanExecuteJavascriptInMainFrame)
                        GetBrowser().MainFrame.ExecuteJavaScriptAsync(value ? BrowserSetting.StyleSheetSetJs : BrowserSetting.StyleSheetUnsetJs);
                }
            }
        }
    }
}
