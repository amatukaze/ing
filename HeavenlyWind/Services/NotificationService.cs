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
using System.Text;
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
                    {
                        rFleet.ExpeditionStatus.Returned += (rpFleetName, rpExpeditionName) =>
                        {
                            if (Preference.Current.Notification.Expedition)
                                Show(StringResources.Instance.Main.Notification_Expedition, string.Format(StringResources.Instance.Main.Notification_Expedition_Content, rpFleetName, rpExpeditionName));
                        };
                        rFleet.ConditionRegeneration.Recovered += rpFleet =>
                        {
                            if (Preference.Current.Notification.RecoveryFromFatigue)
                                Show(StringResources.Instance.Main.Notification_RecoveryFromFatigue, string.Format(StringResources.Instance.Main.Notification_RecoveryFromFatigue_Content, rpFleet.Name));
                        };
                        rFleet.AnchorageRepair.InterruptionNotification += () =>
                        {
                            if (Preference.Current.Notification.AnchorageRepair)
                                Show(StringResources.Instance.Main.Notification_AnchorageRepair, StringResources.Instance.Main.Notification_AnchorageRepair_Content);
                        };
                    }
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
            SessionService.Instance.Subscribe("api_get_member/mapinfo", delegate
            {
                var rFleetWithHeavilyDamagedShips = KanColleGame.Current.Port.Fleets.Table.Values.Where(r => (r.State & FleetState.HeavilyDamaged) == FleetState.HeavilyDamaged);
                if (Preference.Current.Notification.HeavilyDamagedWarning && rFleetWithHeavilyDamagedShips.Any())
                {
                    ShowHeavilyDamagedWarning(StringResources.Instance.Main.Notification_HeavilyDamagedWarning, StringResources.Instance.Main.Notification_HeavilyDamagedWarning_Content, rFleetWithHeavilyDamagedShips.SelectMany(r => r.Ships));
                    FlashWindow();
                }
            });
            SessionService.Instance.Subscribe(new[] { "api_req_sortie/battleresult", "api_req_combined_battle/battleresult" }, delegate
            {
                var rBattle = BattleInfo.Current.CurrentStage;

                var rHeavilyDamagedShips = rBattle.Friend.Where(r => r.State == BattleParticipantState.HeavilyDamaged).Select(r => ((FriendShip)r.Participant).Ship).ToArray();
                if (Preference.Current.Notification.HeavilyDamagedWarning && rHeavilyDamagedShips.Length > 0)
                {
                    ShowHeavilyDamagedWarning(StringResources.Instance.Main.Notification_HeavilyDamagedWarning, StringResources.Instance.Main.Notification_HeavilyDamagedWarning_Content, rHeavilyDamagedShips);
                    FlashWindow();
                }
            });

            SessionService.Instance.Subscribe(new[] { "api_req_map/start", "api_req_map/next" }, delegate
            {
                var rSortie = SortieInfo.Current;
                var rParticipants = rSortie.Fleet.Ships.Skip(1);
                if (rSortie.EscortFleet != null)
                    rParticipants = rParticipants.Concat(rSortie.EscortFleet.Ships.Skip(1));

                var rHeavilyDamagedShips = rParticipants.Where(r => r.State == ShipState.HeavilyDamaged && !r.EquipedEquipment.Any(rpEquipment => rpEquipment.Info.Type == EquipmentType.DamageControl)).ToArray();
                if (Preference.Current.Notification.HeavilyDamagedWarning && rHeavilyDamagedShips.Length > 0)
                {
                    ShowHeavilyDamagedWarning(StringResources.Instance.Main.Notification_AdvanceWarning, StringResources.Instance.Main.Notification_AdvanceWarning_Content, rHeavilyDamagedShips);
                    FlashWindow();
                }
            });
        }

        public void Show(string rpTitle, string rpBody) => ShowCore(rpTitle, rpBody, Preference.Current.Notification.Sound, Preference.Current.Notification.SoundFilename);
        public void ShowHeavilyDamagedWarning(string rpTitle, string rpBody, IEnumerable<Ship> rpHeavilyDamagedShips)
        {
            var rBuilder = new StringBuilder(64);
            foreach (var rShip in rpHeavilyDamagedShips)
            {
                if (rBuilder.Length > 0)
                    rBuilder.Append(" / ");

                rBuilder.Append(rShip.Info.TranslatedName).Append(' ').Append("Lv.").Append(rShip.Level);
            }

            ShowCore(rpTitle, rpBody, Preference.Current.Notification.HeavilyDamagedWarningSound, Preference.Current.Notification.HeavilyDamagedWarningSoundFilename, rBuilder.ToString());
        }
        void ShowCore(string rpTitle, string rpBody, NotificationSound rpSound, string rpCustomSoundFilename, string rpSecondLine = null)
        {
            if (!OS.IsWin8OrLater)
            {
                var rBody = rpBody;
                if (rpSecondLine != null)
                    rBody = $"{rBody}{Environment.NewLine}{rpSecondLine}";

                r_NotifyIcon.ShowBalloonTip(1000, rpTitle, rBody, ToolTipIcon.None);

                if (rpSound == NotificationSound.SystemSound)
                    NativeMethods.WinMM.PlaySoundW("SystemNotification", IntPtr.Zero, NativeEnums.SND.SND_ALIAS | NativeEnums.SND.SND_ASYNC);
            }
            else
            {
                var rToast = new ToastContent()
                {
                    Title = rpTitle,
                    Body = rpBody,
                    BodySecondLine = rpSecondLine,
                    Audio = rpSound == NotificationSound.SystemSound ? ToastAudio.Default : ToastAudio.None,
                };

                ToastNotificationUtil.Show(rToast);
            }

            if (rpSound == NotificationSound.Custom)
                DispatcherUtil.UIDispatcher.BeginInvoke(new Action<string>(PlayCustomSound), rpCustomSoundFilename);
        }
        void PlayCustomSound(string rpCustomSoundFilename)
        {
            if (r_CustomSound == null || r_CustomSound.Item1 != rpCustomSoundFilename)
            {
                Uri rUri;
                if (!Uri.TryCreate(rpCustomSoundFilename, UriKind.RelativeOrAbsolute, out rUri))
                    return;

                var rMediaPlayer = new MediaPlayer();
                rMediaPlayer.Open(rUri);

                r_CustomSound = Tuple.Create(rpCustomSoundFilename, rMediaPlayer);
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
