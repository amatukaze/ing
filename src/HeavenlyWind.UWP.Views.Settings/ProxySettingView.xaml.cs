using Sakuno.KanColle.Amatsukaze.Composition;
using Sakuno.KanColle.Amatsukaze.Settings;
using Windows.UI.Xaml.Controls;

namespace Sakuno.KanColle.Amatsukaze.UWP.Views.Settings
{
    public sealed partial class ProxySettingView : UserControl
    {
        private ProxySetting Instance = StaticResolver.Instance.Resolve<ProxySetting>();
        public ProxySettingView()
        {
            this.InitializeComponent();
        }
    }
}
