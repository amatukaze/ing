using Sakuno.ING.Game.Notification;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;
using Windows.UI.Xaml.Controls;

namespace Sakuno.ING.Views.UWP.Settings
{
    [ExportSettingView(SettingCategory.Notification)]
    public sealed partial class NotificationSettingView : UserControl
    {
        private readonly NotificationManager Manager;

        public NotificationSettingView(NotificationManager manager)
        {
            Manager = manager;
            InitializeComponent();
        }
    }
}
