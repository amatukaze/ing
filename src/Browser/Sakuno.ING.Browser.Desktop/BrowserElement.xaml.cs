using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
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
                _browser?.Navigate(selector.Settings.DefaultUrl.Value);

                layoutSetting.LayoutScale.ValueChanged += _ => UpdateScale();
                layoutSetting.BrowserScale.ValueChanged += _ => UpdateScale();
                Loaded += (s, e) => UpdateScale();

                this.layoutSetting = layoutSetting;
            }
            Application.Current.Exit += OnApplicationExit;
        }

        private void UpdateScale()
        {
            const int GameWidth = 1200;
            const int GameHeight = 720;
            double scale = 1 / layoutSetting.LayoutScale.Value;
            var transform = new ScaleTransform(scale, scale);
            transform.Freeze();
            ActualContent.LayoutTransform = transform;
            ActualContent.Width = GameWidth * layoutSetting.BrowserScale.Value;
            ActualContent.Height = GameHeight * layoutSetting.BrowserScale.Value;
            _browser?.ScaleTo(layoutSetting.BrowserScale.Value);
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            _browserProvider?.Dispose();
        }
    }
}
