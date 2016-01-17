using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.SystemInterop;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    class NotificationService
    {
        const string AppUserModelID = "Sakuno.KanColleInspector";

        public static NotificationService Instance { get; } = new NotificationService();

        IDisposable r_InitializationSubscription, r_ExplorationSubscription;

        public void Initialize()
        {
            ToastNotificationUtil.Initialize("KanColleInspector.lnk", typeof(App).Assembly.Location, AppUserModelID);

            var rGamePropertyChangedSource = Observable.FromEventPattern<PropertyChangedEventArgs>(KanColleGame.Current, nameof(KanColleGame.Current.PropertyChanged))
                .Select(r => r.EventArgs.PropertyName);

            r_InitializationSubscription = rGamePropertyChangedSource.Where(r => r == nameof(KanColleGame.Current.IsStarted)).Subscribe(delegate
            {
                var rPort = KanColleGame.Current.Port;

                var rPortPropertyChangedSource = Observable.FromEventPattern<PropertyChangedEventArgs>(rPort, nameof(rPort.PropertyChanged))
                    .Select(r => r.EventArgs.PropertyName);

                rPort.Fleets.FleetsUpdated += rpFleets =>
                {
                    foreach (var rFleet in rpFleets)
                        rFleet.ExpeditionStatus.Returned += (rpFleetName, rpExpeditionName) =>
                        {
                            var rToast = new ToastContent()
                            {
                                Title = StringResources.Instance.Main.Notification_Expedition,
                                Body = string.Format(StringResources.Instance.Main.Notification_Expedition_Content, rpFleetName, rpExpeditionName),
                                Audio = ToastAudio.Default,
                            };

                            ToastNotificationUtil.Show(rToast);
                        };
                };

                rPortPropertyChangedSource.Where(r => r == nameof(rPort.ConstructionDocks)).Subscribe(delegate
                {
                    foreach (var rConstructionDock in rPort.ConstructionDocks.Values)
                        rConstructionDock.ConstructionCompleted += rpShipName =>
                        {
                            var rToast = new ToastContent()
                            {
                                Title = StringResources.Instance.Main.Notification_Construction,
                                Body = string.Format(StringResources.Instance.Main.Notification_Construction_Content, rpShipName),
                                Audio = ToastAudio.Default,
                            };

                            ToastNotificationUtil.Show(rToast);
                        };
                });
                rPortPropertyChangedSource.Where(r => r == nameof(rPort.RepairDocks)).Subscribe(delegate
                {
                    foreach (var rRepairDock in rPort.RepairDocks.Values)
                        rRepairDock.RepairCompleted += rpShipName =>
                        {
                            var rToast = new ToastContent()
                            {
                                Title = StringResources.Instance.Main.Notification_Repair,
                                Body = string.Format(StringResources.Instance.Main.Notification_Repair_Content, rpShipName),
                                Audio = ToastAudio.Default,
                            };

                            ToastNotificationUtil.Show(rToast);
                        };
                });

                r_InitializationSubscription.Dispose();
                r_InitializationSubscription = null;
            });
        }
    }
}
