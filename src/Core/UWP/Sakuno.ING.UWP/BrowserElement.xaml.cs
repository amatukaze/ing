using System;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Sakuno.ING.UWP
{
    [ExportView("Browser")]
    internal sealed partial class BrowserElement : UserControl
    {
        public readonly WebView WebView;
        private readonly LayoutSetting LayoutSetting;
        private readonly Uri defaultUrl;

        public BrowserElement(UWPHttpProviderSelector selector, LayoutSetting layoutSetting)
        {
            defaultUrl = new Uri(selector.Settings.DefaultUrl.Value);
            LayoutSetting = layoutSetting;
            InitializeComponent();

            if (selector.Settings.Debug.InitialValue)
            {
                Visibility = Visibility.Collapsed;
            }
            else
            {
                Transformer.Child = WebView = new WebView(WebViewExecutionMode.SeparateThread);
                WebView.NavigationStarting += (s, e) => AddressBox.Text = e.Uri.ToString();
                WebView.NavigationCompleted += (s, e) =>
                {
                    if (LockGame)
                        _ = s.InvokeScriptAsync("eval", new[] { BrowserSetting.StyleSheetSetJs });
                };
                WebView.FrameNavigationStarting += (s, e) =>
                {
                    if (LockGame)
                        _ = s.InvokeScriptAsync("eval", new[] { BrowserSetting.StyleSheetSetJs });
                };
                WebView.WebResourceRequested += ((EdgeHttpProvider)selector.Current).WebResourceRequested;
                WebView.Navigate(defaultUrl);

                layoutSetting.LayoutScale.ValueChanged += _ => UpdateBrowserScale();
                layoutSetting.BrowserScale.ValueChanged += _ => UpdateBrowserScale();
                UpdateBrowserScale();
            }
        }

        private bool _lockGame = true;
        private bool LockGame
        {
            get => _lockGame;
            set
            {
                if (_lockGame != value)
                {
                    _lockGame = value;
                    _ = WebView.InvokeScriptAsync("eval", new[] { value ? BrowserSetting.StyleSheetSetJs : BrowserSetting.StyleSheetUnsetJs });
                }
            }
        }

        private void Goto(object sender, RoutedEventArgs e) => WebView.Navigate(new Uri(AddressBox.Text));

        private void GoHome(object sender, RoutedEventArgs e) => WebView.Navigate(defaultUrl);

        private void UpdateBrowserScale()
        {
            float scale = LayoutSetting.BrowserScale.Value / LayoutSetting.LayoutScale.Value;
            Transformer.Transform = new ScaleTransform { ScaleX = scale, ScaleY = scale };
            Transformer.Width = BrowserSetting.Width * scale;
            Transformer.Height = BrowserSetting.Height * scale;
            Width = BrowserSetting.Width * scale;
        }

        private void ClearCache(object sender, RoutedEventArgs e) => _ = WebView.ClearTemporaryWebDataAsync();
    }
}
