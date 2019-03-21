using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Sakuno.ING.Http;
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
        private DpiScale dpi;

        public BrowserElement(BrowserSelector selector, LayoutSetting layoutSetting)
        {
            InitializeComponent();

            if (selector.Settings.Debug.Value || selector.Settings.BrowserEngine.Value == "ProxyOnly")
                Visibility = Visibility.Collapsed;

            if (selector.Settings.Debug.InitialValue)
            {
                var btn = new Button
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    Content = "Next debug data"
                };

                var debug = (DebugHttpProvider)selector.Current;
                btn.Click += (s, e) => debug.Send();

                ActualContent.Content = btn;
            }
            else
            {
                _browserProvider = selector.SelectedBrowser;

                ActualContent.Content = DataContext = _browser = _browserProvider.CreateBrowser();
                _browser?.Navigate(selector.Settings.DefaultUrl.Value);
            }

            layoutSetting.LayoutScale.ValueChanged += _ => UpdateScale();
            layoutSetting.BrowserScale.ValueChanged += _ => UpdateScale();
            Loaded += (s, e) =>
            {
                dpi = VisualTreeHelper.GetDpi(this);
                UpdateScale();
            };

            Application.Current.Exit += OnApplicationExit;
            this.layoutSetting = layoutSetting;
        }

        protected override void OnDpiChanged(DpiScale oldDpi, DpiScale newDpi)
        {
            base.OnDpiChanged(oldDpi, newDpi);
            dpi = newDpi;
        }

        private void UpdateScale()
        {
            double scale = layoutSetting.BrowserScale.Value / (layoutSetting.LayoutScale.Value * dpi.DpiScaleX);
            var transform = new ScaleTransform(scale, scale);
            transform.Freeze();
            ActualContent.LayoutTransform = transform;
            _browser?.ScaleTo(3);
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            _browserProvider?.Dispose();
        }
    }
}
