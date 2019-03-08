using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Notification;
using Sakuno.ING.Settings;

namespace Sakuno.ING.Game.Notification
{
    [Export(typeof(NotificationManager), LazyCreate = false)]
    public sealed class NotificationManager
    {
        public ISettingItem<string> Notifier { get; }
        public IBindableCollection<string> AvailableNotifiers { get; }
        public INotifier SelectedNotifier { get; private set; }
        private readonly INotifier[] notifiers;

        public NotificationManager(INotifier[] notifiers, ISettingsManager settings, NavalBase navalBase)
        {
            this.notifiers = notifiers.Where(x => x.IsSupported).ToArray();
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
