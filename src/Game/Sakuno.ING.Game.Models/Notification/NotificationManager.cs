using System;
using System.Collections.Generic;
using System.Linq;
using Sakuno.ING.Composition;
using Sakuno.ING.Game.Models;
using Sakuno.ING.Localization;
using Sakuno.ING.Notification;
using Sakuno.ING.Settings;
using Sakuno.ING.Shell;

namespace Sakuno.ING.Game.Notification
{
    [Export(typeof(NotificationManager))]
    public sealed class NotificationManager
    {
        public ISettingItem<string> Notifier { get; }
        public IReadOnlyCollection<string> AvailableNotifiers { get; }
        public INotifier SelectedNotifier { get; private set; }
        private readonly INotifier[] notifiers;
        private readonly ILocalizationService localization;
        private readonly bool translate;

        public NotificationManager(INotifier[] notifiers, ISettingsManager settings, ILocalizationService localization, IShell shell, LocaleSetting localeSetting)
        {
            translate = localeSetting.TranslateContent.InitialValue;
            this.notifiers = notifiers.Where(x => x.IsSupported).ToArray();
            AvailableNotifiers = this.notifiers.Select(x => x.Id).ToArray();
            Notifier = settings.Register<string>("notification.notifier");
            Notifier.ValueChanged += SetNotifier;
            SetNotifier(Notifier.Value);
            this.localization = localization;

            shell.Exited += () => SelectedNotifier.Deinitialize();
        }

        private void SetNotifier(string id)
        {
            SelectedNotifier?.Deinitialize();
            SelectedNotifier = notifiers.FirstOrDefault(x => x.Id == id)
                ?? notifiers.FirstOrDefault();
            SelectedNotifier?.Initialize();
        }

        private string TryTranslate(TextTranslationDescriptor name)
        {
            if (name is null) return null;

            if (translate)
                return localization.GetLocalized(name.Category, name.Id.ToString()) ?? name.Origin;
            else
                return name.Origin;
        }

        public void SetExpeditionCompletion(HomeportFleet fleet, DateTimeOffset? timeStamp)
        {
            SelectedNotifier.RemoveSchedule("Expedition" + fleet.Id);
            if (fleet.Expedition != null && timeStamp is DateTimeOffset t)
                SelectedNotifier.AddSchedule("Expedition" + fleet.Id,
                    localization.GetLocalized("Notification", "Expedition_Title"),
                    string.Format(localization.GetLocalized("Notification", "Expedition_Content"),
                        fleet.Id, fleet.Name, fleet.Expedition.DisplayId, TryTranslate(fleet.Expedition.Name)), t - TimeSpan.FromMinutes(1));
        }

        public void SetRepairCompletion(RepairingDock dock, DateTimeOffset? timeStamp)
        {
            SelectedNotifier.RemoveSchedule("Repair" + dock.Id);
            if (dock.RepairingShip != null && timeStamp is DateTimeOffset t)
                SelectedNotifier.AddSchedule("Repair" + dock.Id,
                    localization.GetLocalized("Notification", "Repair_Title"),
                    string.Format(localization.GetLocalized("Notification", "Repair_Content"),
                        dock.Id, TryTranslate(dock.RepairingShip.Info?.Name.FullName)), t - TimeSpan.FromMinutes(1));
        }

        public void SetBuildCompletion(BuildingDock dock, DateTimeOffset? timeStamp)
        {
            SelectedNotifier.RemoveSchedule("Build" + dock.Id);
            if (dock.BuiltShip != null && timeStamp is DateTimeOffset t)
                SelectedNotifier.AddSchedule("Build" + dock.Id,
                    localization.GetLocalized("Notification", "Build_Title"),
                    string.Format(localization.GetLocalized("Notification", "Build_Content"),
                        dock.Id, TryTranslate(dock.BuiltShip?.Name.FullName)), t);
        }
    }
}
