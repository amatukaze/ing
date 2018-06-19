using System.Windows.Controls;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Views.Desktop.Settings
{
    [ExportSettingView(SettingCategory.Network)]
    public partial class ProxySettingView : UserControl
    {
        public ProxySettingView()
        {
            InitializeComponent();
        }
    }
}
