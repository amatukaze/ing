using System.Windows;
using System.Windows.Controls;
using Sakuno.ING.Http;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Browser.Desktop
{
    [ExportView("Browser")]
    internal partial class BrowserElement : UserControl
    {
        private readonly IBrowserProvider _browserProvider;
        private readonly IBrowser _browser;

        public BrowserElement(BrowserSelector selector)
        {
            InitializeComponent();

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

            Application.Current.Exit += OnApplicationExit;
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            _browserProvider?.Dispose();
        }
    }
}
