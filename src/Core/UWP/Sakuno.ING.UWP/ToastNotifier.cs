using Windows.UI.Notifications;

namespace Sakuno.ING.UWP
{
    internal partial class ToastNotifier
    {
        public bool IsSupported => true;
        public void Initialize() => ToastNotificationManager.History.Clear();
        private Windows.UI.Notifications.ToastNotifier CreateNotifier() => ToastNotificationManager.CreateToastNotifier();
    }
}
