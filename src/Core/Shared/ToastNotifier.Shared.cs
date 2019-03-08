using Microsoft.Toolkit.Uwp.Notifications;
using Sakuno.ING.Composition;
using Sakuno.ING.Notification;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

#if WINDOWS_UWP
namespace Sakuno.ING.UWP
#elif WPF
namespace Sakuno.ING.Shell.Desktop
#endif
{
    [Export(typeof(INotifier))]
    internal partial class ToastNotifier : INotifier
    {
        public string Id => "Toast";

        private Windows.UI.Notifications.ToastNotifier notifier;
        public void Show(string title, string content, string sound)
        {
            var toastContent = new ToastContent
            {
                Visual = new ToastVisual
                {
                    BindingGeneric = new ToastBindingGeneric
                    {
                        Children =
                        {
                            new AdaptiveText { Text = title },
                            new AdaptiveText { Text = content }
                        }
                    }
                }
            };
            var doc = new XmlDocument();
            doc.LoadXml(toastContent.GetContent());
            notifier.Show(new ToastNotification(doc));
        }
    }
}
