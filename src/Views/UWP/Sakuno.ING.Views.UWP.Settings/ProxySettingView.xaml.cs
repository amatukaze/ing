using Sakuno.ING.Composition;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Settings
{
    [ExportSettingView(SettingCategory.Network)]
    public sealed partial class ProxySettingView : UserControl
    {
        private ProxySetting Instance = Compositor.Static<ProxySetting>();
        public ProxySettingView()
        {
            this.InitializeComponent();
        }
    }
}
