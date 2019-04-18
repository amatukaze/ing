using System.Windows.Controls;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Views.Desktop.Settings
{
    [ExportSettingView(SettingCategory.Notification)]
    public partial class NotificationSettingView : UserControl
    {
        public NotificationSettingView()
        {
            InitializeComponent();
        }
    }
}
