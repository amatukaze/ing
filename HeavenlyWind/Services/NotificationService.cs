using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.SystemInterop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Forms;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    class NotificationService
    {
        const string AppUserModelID = "Sakuno.Amatsukaze";

        public static NotificationService Instance { get; } = new NotificationService();

        NotifyIcon r_NotifyIcon;

        PropertyChangedEventListener r_SortiePCEL;

        public void Initialize()
        {
            if (OS.IsWin8OrLater)
                InstallShortcut();
            else
                InitializeNotifyIcon();

            var rGamePCEL = PropertyChangedEventListener.FromSource(KanColleGame.Current);
            rGamePCEL.Add(nameof(KanColleGame.Current.IsStarted), delegate
            {
                var rPort = KanColleGame.Current.Port;
                rPort.Fleets.FleetsUpdated += rpFleets =>
                {
                    foreach (var rFleet in rpFleets)
                        rFleet.ExpeditionStatus.Returned += (rpFleetName, rpExpeditionName) =>
                        {
                            if (Preference.Current.Notification.Expedition)
                                Show(StringResources.Instance.Main.Notification_Expedition, string.Format(StringResources.Instance.Main.Notification_Expedition_Content, rpFleetName, rpExpeditionName));
                        };
                };

                var rPortPCEL = PropertyChangedEventListener.FromSource(rPort);
                rPortPCEL.Add(nameof(rPort.ConstructionDocks), delegate
                {
                    foreach (var rConstructionDock in rPort.ConstructionDocks.Values)
                        rConstructionDock.ConstructionCompleted += rpShipName =>
                        {
                            if (Preference.Current.Notification.Construction)
                                Show(StringResources.Instance.Main.Notification_Construction, string.Format(StringResources.Instance.Main.Notification_Construction_Content, rpShipName));
                        };
                });
                rPortPCEL.Add(nameof(rPort.RepairDocks), delegate
                {
                    foreach (var rRepairDock in rPort.RepairDocks.Values)
                        rRepairDock.RepairCompleted += rpShipName =>
                        {
                            if (Preference.Current.Notification.Repair)
                                Show(StringResources.Instance.Main.Notification_Repair, string.Format(StringResources.Instance.Main.Notification_Repair_Content, rpShipName));
                        };
                });
            });

            InitializeHeavilyDamagedWarning(rGamePCEL);
        }
        static void InstallShortcut()
        {
            var rOldShortcut = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Programs), "KanColleInspector.lnk"));
            if (rOldShortcut.Exists)
                rOldShortcut.Delete();

            string rShortcutName;
            var rCultures = StringResources.GetAncestorsAndSelfCultureNames(CultureInfo.CurrentCulture).ToArray();
            if (rCultures.Any(r => r.OICEquals("ja")))
                rShortcutName = "いんてりじぇんと連装砲くん.lnk";
            else if (rCultures.Any(r => r.OICEquals("zh-Hans")))
                rShortcutName = "智能型连装炮君.lnk";
            else if (rCultures.Any(r => r.OICEquals("zh-Hant")))
                rShortcutName = "智能型連裝炮君.lnk";
            else
                rShortcutName = "Intelligent Naval Gun.lnk";

            ToastNotificationUtil.Initialize(rShortcutName, typeof(App).Assembly.Location, AppUserModelID);
        }
        void InitializeNotifyIcon()
        {
            var rResourceInfo = App.GetResourceStream(new Uri("pack://application:,,,/HeavenlyWind;component/app.ico"));
            if (rResourceInfo == null)
                return;

            using (var rStream = rResourceInfo.Stream)
                r_NotifyIcon = new NotifyIcon()
                {
                    Text = StringResources.Instance.Main.Product_Name,
                    Icon = new Icon(rStream),
                    Visible = true,
                };
        }

        void InitializeHeavilyDamagedWarning(PropertyChangedEventListener rpGamePCEL)
        {
            SessionService.Instance.Subscribe(new[] { "api_req_sortie/battleresult", "api_req_combined_battle/battleresult" }, delegate
            {
                var rBattle = BattleInfo.Current.CurrentStage;

                IEnumerable<BattleParticipantSnapshot> rParticipants = rBattle.FriendMain;
                if (rBattle.FriendEscort != null)
                    rParticipants = rParticipants.Concat(rBattle.FriendEscort);

                if (Preference.Current.Notification.HeavilyDamagedWarning && rParticipants.Any(r => r.State == BattleParticipantState.HeavilyDamaged))
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

                        if (Preference.Current.Notification.HeavilyDamagedWarning && rParticipants.Any(r => r.State == ShipState.HeavilyDamaged && !r.EquipedEquipment.Any(rpEquipment => rpEquipment.Info.Type == EquipmentType.DamageControl)))
                            Show(StringResources.Instance.Main.Notification_AdvanceWarning, StringResources.Instance.Main.Notification_AdvanceWarning_Content);
                    });
                }
            });
        }

        public void Show(string rpTitle, string rpBody)
        {
            if (!OS.IsWin8OrLater)
                r_NotifyIcon.ShowBalloonTip(1000, rpTitle, rpBody, ToolTipIcon.None);
            else
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
}
