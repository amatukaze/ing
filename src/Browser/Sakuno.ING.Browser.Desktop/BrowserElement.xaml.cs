using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Browser.Desktop
{
    [ExportView("Browser")]
    internal partial class BrowserElement : UserControl
    {
        private readonly IBrowserProvider _browserProvider;
        private readonly IBrowser _browser;
        private readonly LayoutSetting layoutSetting;

        public BrowserElement(BrowserSelector selector, LayoutSetting layoutSetting)
        {
            InitializeComponent();

            _browserProvider = selector.SelectedBrowser;
            ActualContent.Content = DataContext = _browser = _browserProvider?.CreateBrowser();

            if (_browser is null)
            {
                Visibility = Visibility.Collapsed;
            }
            else
            {
                _browser.LockGame = true;
                _browser.Navigate(selector.Settings.DefaultUrl.Value);

                layoutSetting.LayoutScale.ValueChanged += _ => UpdateScale();
                layoutSetting.BrowserScale.ValueChanged += _ => UpdateScale();
                Loaded += (s, e) => UpdateScale();

                this.layoutSetting = layoutSetting;
            }
            Application.Current.Exit += OnApplicationExit;
        }

        private void UpdateScale()
        {
            double browserScale = layoutSetting.BrowserScale.Value;
            double sizeScale = browserScale / layoutSetting.LayoutScale.Value;
            ActualContent.Width = BrowserSetting.Width * sizeScale;
            ActualContent.Height = BrowserSetting.Height * sizeScale;
            this.Width = BrowserSetting.Width * sizeScale;
            _browser?.ScaleTo(browserScale);
        }

        private void ToggleGameLock(object sender, RoutedEventArgs e)
        {
            if (_browser != null)
                _browser.LockGame = ((ToggleButton)sender).IsChecked ?? false;
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            _browserProvider?.Dispose();
        }
    }
}
