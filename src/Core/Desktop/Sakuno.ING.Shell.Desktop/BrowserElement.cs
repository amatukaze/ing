using System.Windows;
using System.Windows.Controls;
using Sakuno.ING.Browser.Desktop;

namespace Sakuno.ING.Shell.Desktop
{
    [ExportView("Browser")]
    internal class BrowserElement : ContentControl
    {
        IBrowserProvider _browserProvider;

        public BrowserElement(BrowserSelector selector)
        {
            _browserProvider = selector.SelectedBrowser;

            Content = _browserProvider.CreateBrowser();

            Application.Current.Exit += OnApplicationExit;
        }

        void OnApplicationExit(object sender, ExitEventArgs e)
        {
            _browserProvider.Dispose();
        }
    }
}
