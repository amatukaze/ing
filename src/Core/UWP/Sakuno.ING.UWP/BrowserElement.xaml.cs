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
        private readonly LayoutSetting layoutSetting;

        public BrowserElement(UWPHttpProviderSelector selector, LayoutSetting layoutSetting)
        {
            this.layoutSetting = layoutSetting;
            this.InitializeComponent();

            if (selector.Settings.Debug.InitialValue)
            {
                Visibility = Visibility.Collapsed;
            }
            else
            {
                Transformer.Child = WebView = new WebView(WebViewExecutionMode.SeparateThread);
                WebView.Navigate(new Uri(selector.Settings.DefaultUrl.Value));

                layoutSetting.LayoutScale.ValueChanged += _ => UpdateBrowserScale();
                layoutSetting.BrowserScale.ValueChanged += _ => UpdateBrowserScale();
                UpdateBrowserScale();
            }
        }

        private void UpdateBrowserScale()
        {
            float scale = layoutSetting.BrowserScale.Value / layoutSetting.LayoutScale.Value;
            Transformer.Transform = new ScaleTransform { ScaleX = scale, ScaleY = scale };
        }
    }
}
