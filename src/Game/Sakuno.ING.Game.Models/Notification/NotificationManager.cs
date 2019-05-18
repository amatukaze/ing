using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Notification;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Game.Notification
{
    [Export(typeof(NotificationManager))]
    public sealed class NotificationManager
    {
        public ISettingItem<string> Notifier { get; }
        public IReadOnlyCollection<string> AvailableNotifiers { get; }
        public INotifier SelectedNotifier { get; private set; }
        private readonly INotifier[] notifiers;

        public NotificationManager(INotifier[] notifiers, ISettingsManager settings)
        {
            this.notifiers = notifiers.Where(x => x.IsSupported).ToArray();
            AvailableNotifiers = this.notifiers.Select(x => x.Id).ToArray();
            Notifier = settings.Register<string>("notification.notifier");
            Notifier.ValueChanged += SetNotifier;
            SetNotifier(Notifier.Value);
        }

        private void SetNotifier(string id)
        {
            SelectedNotifier = notifiers.FirstOrDefault(x => x.Id == id)
                ?? notifiers.FirstOrDefault();
            SelectedNotifier?.Initialize();
        }
    }
}
