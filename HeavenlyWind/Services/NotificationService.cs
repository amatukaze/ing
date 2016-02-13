using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.SystemInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    class NotificationService
    {
        const string AppUserModelID = "Sakuno.KanColleInspector";

        public static NotificationService Instance { get; } = new NotificationService();

        PropertyChangedEventListener r_SortiePCEL;

        public void Initialize()
        {
            if (!OS.IsWin8OrLater)
                return;

            ToastNotificationUtil.Initialize("KanColleInspector.lnk", typeof(App).Assembly.Location, AppUserModelID);

            var rGamePCEL = PropertyChangedEventListener.FromSource(KanColleGame.Current);
            rGamePCEL.Add(nameof(KanColleGame.Current.IsStarted), delegate
            {
                var rPort = KanColleGame.Current.Port;
                rPort.Fleets.FleetsUpdated += rpFleets =>
                {
                    foreach (var rFleet in rpFleets)
                        rFleet.ExpeditionStatus.Returned += (rpFleetName, rpExpeditionName) =>
                            Show(StringResources.Instance.Main.Notification_Expedition, string.Format(StringResources.Instance.Main.Notification_Expedition_Content, rpFleetName, rpExpeditionName));
                };

                var rPortPCEL = PropertyChangedEventListener.FromSource(rPort);
                rPortPCEL.Add(nameof(rPort.ConstructionDocks), delegate
                {
                    foreach (var rConstructionDock in rPort.ConstructionDocks.Values)
                        rConstructionDock.ConstructionCompleted += rpShipName =>
                            Show(StringResources.Instance.Main.Notification_Construction, string.Format(StringResources.Instance.Main.Notification_Construction_Content, rpShipName));
                });
                rPortPCEL.Add(nameof(rPort.RepairDocks), delegate
                {
                    foreach (var rRepairDock in rPort.RepairDocks.Values)
                        rRepairDock.RepairCompleted += rpShipName =>
                            Show(StringResources.Instance.Main.Notification_Repair, string.Format(StringResources.Instance.Main.Notification_Repair_Content, rpShipName));
                });
            });

            InitializeHeavilyDamagedWarning(rGamePCEL);
        }

        void InitializeHeavilyDamagedWarning(PropertyChangedEventListener rpGamePCEL)
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

            rpGamePCEL.Add(nameof(KanColleGame.Current.Sortie), delegate
            {
                var rSortie = KanColleGame.Current.Sortie;

                if (rSortie == null)
                {
                    r_SortiePCEL?.Dispose();
                    r_SortiePCEL = null;
                }
                else
                {
                    r_SortiePCEL = new PropertyChangedEventListener(rSortie);
                    r_SortiePCEL.Add(nameof(rSortie.Node), delegate
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
