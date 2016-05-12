using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
using Sakuno.KanColle.Amatsukaze.Game.Models.Battle;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.SystemInterop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Reactive.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    class NotificationService
    {
        const string AppUserModelID = "Sakuno.Amatsukaze";

        public static NotificationService Instance { get; } = new NotificationService();

        NotifyIcon r_NotifyIcon;

        Tuple<string, MediaPlayer> r_CustomSound;

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

                if (Preference.Current.Notification.HeavilyDamagedWarning && rBattle.Friend.Any(r => r.State == BattleParticipantState.HeavilyDamaged))
                {
                    Show(StringResources.Instance.Main.Notification_HeavilyDamagedWarning, StringResources.Instance.Main.Notification_HeavilyDamagedWarning_Content);
                    FlashWindow();
                }
            });

            SessionService.Instance.Subscribe(new[] { "api_req_map/start", "api_req_map/next" }, delegate
            {
                var rSortie = SortieInfo.Current;
                var rParticipants = rSortie.Fleet.Ships.Skip(1);
                if (rSortie.EscortFleet != null)
                    rParticipants = rParticipants.Concat(rSortie.EscortFleet.Ships.Skip(1));

                if (Preference.Current.Notification.HeavilyDamagedWarning && rParticipants.Any(r => r.State == ShipState.HeavilyDamaged && !r.EquipedEquipment.Any(rpEquipment => rpEquipment.Info.Type == EquipmentType.DamageControl)))
                {
                    Show(StringResources.Instance.Main.Notification_AdvanceWarning, StringResources.Instance.Main.Notification_AdvanceWarning_Content);
                    FlashWindow();
                }
            });
        }

        public void Show(string rpTitle, string rpBody)
        {
            var rSound = Preference.Current.Notification.Sound;

            if (!OS.IsWin8OrLater)
            {
                r_NotifyIcon.ShowBalloonTip(1000, rpTitle, rpBody, ToolTipIcon.None);

                if (rSound == NotificationSound.SystemSound)
                    SystemSounds.Exclamation.Play();
            }
            else
            {
                var rToast = new ToastContent()
                {
                    Title = rpTitle,
                    Body = rpBody,
                    Audio = rSound == NotificationSound.SystemSound ? ToastAudio.Default : ToastAudio.None,
                };

                ToastNotificationUtil.Show(rToast);
            }

            if (rSound == NotificationSound.Custom)
                PlayCustomSound();
        }
        void PlayCustomSound()
        {
            if (r_CustomSound == null || r_CustomSound.Item1 != Preference.Current.Notification.SoundFilename)
            {
                Uri rUri;
                if (!Uri.TryCreate(Preference.Current.Notification.SoundFilename, UriKind.RelativeOrAbsolute, out rUri))
                    return;

                var rMediaPlayer = new MediaPlayer();
                rMediaPlayer.Open(rUri);

                r_CustomSound = Tuple.Create(Preference.Current.Notification.SoundFilename, rMediaPlayer);
            }

            r_CustomSound.Item2.Stop();
            r_CustomSound.Item2.Play();
        }
        void FlashWindow()
        {
            var rHandle = DispatcherUtil.UIDispatcher.Invoke(() => new WindowInteropHelper(App.Current.MainWindow).Handle);
            var rInfo = new NativeStructs.FLASHWINFO()
            {
                cbSize = Marshal.SizeOf(typeof(NativeStructs.FLASHWINFO)),
                hwnd = rHandle,
                dwFlags = NativeEnums.FLASHW.FLASHW_TRAY | NativeEnums.FLASHW.FLASHW_TIMERNOFG,
                dwTimeout = 250,
                uCount = 5,
            };
            NativeMethods.User32.FlashWindowEx(ref rInfo);
        }
    }
}
