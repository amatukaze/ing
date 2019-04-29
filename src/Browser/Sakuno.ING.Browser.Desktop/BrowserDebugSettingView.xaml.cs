using System.Windows.Controls;

namespace Sakuno.ING.Browser.Desktop
{
#if DEBUG
    [Shell.ExportSettingView(Settings.SettingCategory.Browser)]
#endif
    public partial class BrowserDebugSettingView : UserControl
    {
        public BrowserDebugSettingView()
        {
            InitializeComponent();
        }
    }
}
