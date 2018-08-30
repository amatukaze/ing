using System.Windows;

namespace Sakuno.ING.Browser.Desktop
{
    public interface IBrowserProvider
    {
        string Id { get; }

        IBrowserHost CreateBrowser();

        void ClearCache();
        void ClearCookie();
        UIElement CreateSettingsView();
    }
}
