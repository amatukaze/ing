using System.Windows.Controls;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Browser.Desktop
{
    [ExportSettingView(SettingCategory.Browser)]
    public partial class BrowserSettingView : UserControl
    {
        public BrowserSettingView()
        {
            InitializeComponent();
        }
    }
}
