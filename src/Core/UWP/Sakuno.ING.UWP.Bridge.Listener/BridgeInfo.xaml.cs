using Sakuno.ING.Services;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.UWP.Bridge
{
    [ExportSettingView(SettingCategory.Network)]
    public sealed partial class BridgeInfo : UserControl
    {
        private string Version = Constants.Version;
        private readonly Provider Provider;
        public BridgeInfo(ITextStreamProvider provider)
        {
            Provider = (Provider)provider;
            this.InitializeComponent();
        }

        private Visibility Not(bool value) => value ? Visibility.Collapsed : Visibility.Visible;
    }
}
