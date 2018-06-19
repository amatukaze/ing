using System.Windows.Controls;
using Sakuno.ING.Composition;
using Sakuno.ING.Settings;

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
