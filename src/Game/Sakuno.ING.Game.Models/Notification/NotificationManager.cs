using System.Collections.Generic;
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
        public IReadOnlyCollection<string> AvailableNotifiers { get; }
        public INotifier SelectedNotifier { get; private set; }
        private readonly INotifier[] notifiers;

        public NotificationManager(INotifier[] notifiers, ISettingsManager settings, NavalBase navalBase)
        {
            this.notifiers = notifiers.Where(x => x.IsSupported).ToArray();
            AvailableNotifiers = this.notifiers.Select(x => x.Id).ToArray();
            Notifier = settings.Register<string>("notification.notifier");
            Notifier.ValueChanged += SetNotifier;
            SetNotifier(Notifier.Value);

            navalBase.RepairTiming += r =>
                SelectedNotifier.Show("Repair completing", $"{r.Id}: {r.RepairingShip.Info?.Name.Origin}", null);
            navalBase.BuildTiming += b =>
                SelectedNotifier.Show("Build completing", $"{b.Id}: {b.BuiltShip?.Name.Origin}", null);
            navalBase.ExpeditionTiming += f =>
                SelectedNotifier.Show("Expedition completing", $"{f.Id}: {f.Expedition?.DisplayId}: {f.Expedition?.Name.Origin}", null);
        }

        private void SetNotifier(string id)
        {
            SelectedNotifier = notifiers.FirstOrDefault(x => x.Id == id)
                ?? notifiers.FirstOrDefault();
            SelectedNotifier?.Initialize();
        }
    }
}
