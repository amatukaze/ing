using System.Windows;
using System.Windows.Controls;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Browser.Desktop
{
    [ExportView("Browser")]
    internal class BrowserElement : ContentControl
    {
        private IBrowserProvider _browserProvider;

        public BrowserElement(BrowserSelector selector)
        {
            _browserProvider = selector.SelectedBrowser;

            Content = _browserProvider.CreateBrowser();

            Application.Current.Exit += OnApplicationExit;
        }

        private void OnApplicationExit(object sender, ExitEventArgs e)
        {
            _browserProvider.Dispose();
        }
    }
}
