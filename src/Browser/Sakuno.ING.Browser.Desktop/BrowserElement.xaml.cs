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
        private const double browserWidth = 1200, browserHeight = 720;
        //private DpiScale dpi;

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
                Loaded += (s, e) =>
                {
                    //dpi = VisualTreeHelper.GetDpi(this);
                    UpdateScale();
                };

                this.layoutSetting = layoutSetting;
            }
            Application.Current.Exit += OnApplicationExit;
        }

        //protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        //{
        //    base.OnDpiChanged(oldDpi, newDpi);
        //    dpi = newDpi;
        //}

        private void UpdateScale()
        {
            double browserScale = layoutSetting.BrowserScale.Value;
            double sizeScale = browserScale / layoutSetting.LayoutScale.Value;
            ActualContent.Width = browserWidth * sizeScale;
            ActualContent.Height = browserHeight * sizeScale;
            this.Width = browserWidth * sizeScale;
            _browser?.ScaleTo(browserScale);
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            _browserProvider?.Dispose();
        }
    }
}
