using System;
using Sakuno.ING.Http;
using Sakuno.ING.Shell;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP
{
    [ExportView("Browser")]
    internal sealed partial class BrowserElement : UserControl
    {
        public readonly WebView WebView;
        public readonly UIElement ActualContent;
        public BrowserElement(UWPHttpProviderSelector selector)
        {
            if (selector.Debug.InitialValue)
            {
                var btn = new Button
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Content = "Next debug data"
                };

                var debug = (DebugHttpProvider)selector.Current;
                btn.Tapped += (s, e) => debug.Send();

                ActualContent = btn;
            }
            else
            {
                ActualContent = WebView = new WebView(WebViewExecutionMode.SeparateThread);
                WebView.Navigate(new Uri(selector.DefaultUrl.Value));
            }

            this.InitializeComponent();
        }
    }
}
