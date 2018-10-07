using System.Windows;
using System.Windows.Controls;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Browser.Desktop
{
    [ExportView("Browser")]
    internal class BrowserElement : ContentControl
    {
        private readonly IBrowserProvider _browserProvider;
        private readonly IBrowser _browser;

        public BrowserElement(BrowserSelector selector)
        {
            _browserProvider = selector.SelectedBrowser;

            Content = _browser = _browserProvider.CreateBrowser();
            _browser?.Navigate(selector.DefaultUrl.Value);

            Application.Current.Exit += OnApplicationExit;
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            _browserProvider.Dispose();
        }
    }
}
