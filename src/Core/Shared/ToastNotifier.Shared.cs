using System;
using System.Linq;
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
        private const string defaultGroup = "default";

        public void AddSchedule(string id, string title, string content, DateTimeOffset time)
        {
            if (time <= DateTimeOffset.Now) return;
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
            try
            {
                CreateNotifier().AddToSchedule(new ScheduledToastNotification(doc, time)
                {
                    Tag = id,
                    Group = defaultGroup
                });
            }
            catch { } // Will fail if time goes
        }

        public void RemoveSchedule(string id)
        {
            var notifier = CreateNotifier();
            var scheduled = notifier.GetScheduledToastNotifications().FirstOrDefault(x => x.Tag == id);
            if (scheduled != null)
                notifier.RemoveFromSchedule(scheduled);

            if (ToastNotificationManager.History.GetHistory(
#if WPF
                aumid
#endif
                ).Any(x => x.Tag == id))
            {
                try
                {
                    ToastNotificationManager.History.Remove(id, defaultGroup
#if WPF
                        ,aumid
#endif
                        );
                }
                catch { } // May fail if not exists
            }
        }

        public void Deinitialize() { }
    }
}
