using System.Windows.Controls;

namespace Sakuno.ING.Shell.Desktop
{
    [ExportView("Browser")]
    internal class BrowserElement : ContentControl
    {
        public BrowserElement(BrowserSelector selector)
        {
            Content = selector.SelectedBrowser.CreateBrowser();
        }
    }
}
