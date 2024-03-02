using CefSharp;
using CefSharp.Wpf.HwndHost;
using Sakuno.KanColle.Amatsukaze.Browser.Blink.Handlers;
using Sakuno.KanColle.Amatsukaze.Extensibility.Browser;
using System;
using System.Linq;
using System.Threading.Tasks;
using IBrowser = Sakuno.KanColle.Amatsukaze.Extensibility.Browser.IBrowser;

namespace Sakuno.KanColle.Amatsukaze.Browser.Blink
{
    class BlinkBrowser : ChromiumWebBrowser, IBrowser, IBlinkBrowser
    {
        double _percentage;

        public event Action<bool, bool, string> LoadCompleted;

        public BlinkBrowser()
        {
            MenuHandler = new ContextMenuHandler();
            LifeSpanHandler = new LifeSpanHandler();
            KeyboardHandler = new KeyboardHandler();
            DragHandler = new DragHandler();

            LoadingStateChanged += Browser_LoadingStateChanged;

            FrameLoadEnd += BlinkBrowser_FrameLoadEnd;
        }

        private void BlinkBrowser_FrameLoadEnd(object sender, FrameLoadEndEventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                SetZoom(_percentage);
            });
        }

        void Browser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                SetZoom(_percentage);
                LoadCompleted?.Invoke(e.CanGoBack, e.CanGoForward, Address);
            });
        }

        public void Navigate(string rpUrl) => Load(rpUrl);

        public void SetZoom(double rpPercentage)
        {
            _percentage = rpPercentage;

            GetBrowser().SetZoomLevel(Math.Log(rpPercentage, 1.2));
        }

        public Task<string> TakeScreenshotAsync()
        {
            var request = new ScreenshotRequest();
            JavascriptObjectRepository.Register(request.Id, request, true);

            var script = $@"(async function()
{{
    await CefSharp.BindObjectAsync('{request.Id}');

    let canvas = document.querySelector('canvas');
    requestAnimationFrame(() =>
    {{
        let dataurl = canvas.toDataURL('image/png');
        {request.Id}.complete(dataurl);
    }});
}})();";
            var frame = GetKanColleFrame();
            frame.ExecuteJavaScriptAsync(script);

            request.Task.ContinueWith(delegate
            {
                JavascriptObjectRepository.UnRegister(request.Id);
                frame.ExecuteJavaScriptAsync("delete " + request.Id);
            });

            return request.Task;
        }
        IFrame GetKanColleFrame()
        {
            var browser = GetBrowser();
            var gameFrame = browser.GetFrame("game_frame");
            if (gameFrame == null)
                return null;

            var frames = browser.GetFrameIdentifiers()
                .Select(r => browser.GetFrame(r))
                .Where(r => r.Parent?.Identifier == gameFrame.Identifier);

            return frames.FirstOrDefault(f => f.Url.Contains(@"/kcs2/index.php"));
        }

        public void OnMaxFramerateChanged(int framerate)
        {
            //Dispatcher.InvokeAsync(() =>
            //{
            //    GetBrowser().GetHost().WindowlessFrameRate = framerate;
            //});
        }

        public void GoBack()
        {
            this.WebBrowser.Back();
        }

        public void GoForward()
        {
            this.WebBrowser.Forward();
        }

        public void Refresh()
        {
            this.WebBrowser.Reload();
        }
    }
}
