using Windows.UI.Notifications;

namespace Sakuno.ING.UWP
{
    internal partial class ToastNotifier
    {
        public bool IsSupported => true;

        public void Initialize() => notifier = ToastNotificationManager.CreateToastNotifier();
    }
}
