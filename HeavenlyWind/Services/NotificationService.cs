using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.SystemInterop;
using System;
using System.Collections.Generic;
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
            r_InitializationSubscription = rGamePropertyChangedSource.Where(r => r == nameof(KanColleGame.Current.IsStarted)).Subscribe(_ =>
            {
                var rPort = KanColleGame.Current.Port;

                var rPortPropertyChangedSource = Observable.FromEventPattern<PropertyChangedEventArgs>(rPort, nameof(rPort.PropertyChanged))
                    .Select(r => r.EventArgs.PropertyName);

                rPort.Fleets.FleetsUpdated += rpFleets =>
                {
                    foreach (var rFleet in rpFleets)
                        rFleet.ExpeditionStatus.Returned += (rpFleetName, rpExpeditionName) =>
                            Show(StringResources.Instance.Main.Notification_Expedition, string.Format(StringResources.Instance.Main.Notification_Expedition_Content, rpFleetName, rpExpeditionName));
                };

                rPortPropertyChangedSource.Where(r => r == nameof(rPort.ConstructionDocks)).Subscribe(delegate
                {
                    foreach (var rConstructionDock in rPort.ConstructionDocks.Values)
                        rConstructionDock.ConstructionCompleted += rpShipName =>
                            Show(StringResources.Instance.Main.Notification_Construction, string.Format(StringResources.Instance.Main.Notification_Construction_Content, rpShipName));
                });
                rPortPropertyChangedSource.Where(r => r == nameof(rPort.RepairDocks)).Subscribe(delegate
                {
                    foreach (var rRepairDock in rPort.RepairDocks.Values)
                        rRepairDock.RepairCompleted += rpShipName =>
                            Show(StringResources.Instance.Main.Notification_Repair, string.Format(StringResources.Instance.Main.Notification_Repair_Content, rpShipName));
                });

                r_InitializationSubscription.Dispose();
                r_InitializationSubscription = null;
            });

            InitializeHeavilyDamagedWarning(rGamePropertyChangedSource);
        }
        void InitializeHeavilyDamagedWarning(IObservable<string> rpGamePropertyChangedSource)
        {
            SessionService.Instance.Subscribe(new[] { "api_req_sortie/battleresult", "api_req_combined_battle/battleresult" }, delegate
            {
                var rBattle = BattleInfo.Current.CurrentStage;

                IEnumerable<BattleParticipantSnapshot> rParticipants = rBattle.FriendMain;
                if (rBattle.FriendEscort != null)
                    rParticipants = rParticipants.Concat(rBattle.FriendEscort);

                if (rParticipants.Any(r => r.State == BattleParticipantState.HeavilyDamaged))
                    Show(StringResources.Instance.Main.Notification_HeavilyDamagedWarning, StringResources.Instance.Main.Notification_HeavilyDamagedWarning_Content);
            });

            rpGamePropertyChangedSource.Where(r => r == nameof(KanColleGame.Current.Sortie)).Subscribe(delegate
            {
                var rSortie = KanColleGame.Current.Sortie;

                if (rSortie == null)
                {
                    r_ExplorationSubscription?.Dispose();
                    r_ExplorationSubscription = null;
                }
                else
                {
                    var rSortiePropertyChangedSource = Observable.FromEventPattern<PropertyChangedEventArgs>(rSortie, nameof(rSortie.PropertyChanged))
                        .Select(r => r.EventArgs.PropertyName);

                    r_ExplorationSubscription = rSortiePropertyChangedSource.Where(r => r == nameof(rSortie.Cell)).Subscribe(delegate
                    {
                        var rParticipants = rSortie.Fleet.Ships.Skip(1);
                        if (rSortie.EscortFleet != null)
                            rParticipants = rParticipants.Concat(rSortie.EscortFleet.Ships.Skip(1));

                        if (rParticipants.Any(r => r.State == ShipState.HeavilyDamaged && !r.EquipedEquipment.Any(rpEquipment => rpEquipment.Info.Type == EquipmentType.DamageControl)))
                            Show(StringResources.Instance.Main.Notification_AdvanceWarning, StringResources.Instance.Main.Notification_AdvanceWarning_Content);
                    });
                }
            });
        }

        public void Show(string rpTitle, string rpBody)
        {
            var rToast = new ToastContent()
            {
                Title = rpTitle,
                Body = rpBody,
                Audio = ToastAudio.Default,
            };

            ToastNotificationUtil.Show(rToast);
        }
    }
}
