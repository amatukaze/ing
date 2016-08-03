using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Internal;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.KanColle.Amatsukaze.Views.Tools;
using Sakuno.SystemInterop;
using Sakuno.UserInterface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class GameController : ModelBase
    {
        BrowserService r_Owner;

        bool r_IsAudioDeviceNotAvailable;
        BrowserAudioSession r_AudioSession;
        public BrowserAudioSession AudioSession
        {
            get { return r_AudioSession; }
            private set
            {
                if (r_AudioSession != value)
                {
                    r_AudioSession = value;
                    OnPropertyChanged(nameof(AudioSession));
                }
            }
        }

        public ICommand TakeScreenshotToFileCommand { get; }
        public ICommand TakeScreenshotToClipboardCommand { get; }
        public ICommand OpenScreenshotToolCommand { get; }

        public ICommand MuteToggleCommand { get; }

        public ICommand SetZoomCommand { get; }
        public ICommand ZoomInCommand { get; private set; }
        public ICommand ZoomOutCommand { get; private set; }
        public IList<BrowserZoomInfo> ZoomFactors { get; }
        public double Zoom
        {
            get { return Preference.Instance.Browser.Zoom; }
            set
            {
                if (Zoom != value)
                    SetZoom(value);
            }
        }

        public ICommand RestartGameCommand { get; }

        public GameController(BrowserService rpOwner)
        {
            r_Owner = rpOwner;

            TakeScreenshotToFileCommand = new DelegatedCommand(() => ScreenshotService.Instance.TakeScreenshotAndOutput(rpOutputToClipboard: false));
            TakeScreenshotToClipboardCommand = new DelegatedCommand(() => ScreenshotService.Instance.TakeScreenshotAndOutput(rpOutputToClipboard: true));
            OpenScreenshotToolCommand = new DelegatedCommand(ScreenshotTool.Open);

            if (OS.IsWin7OrLater && !rpOwner.NoInstalledLayoutEngines)
                try
                {
                    AudioManager.Instance.StartSessionNotification();
                    AudioManager.Instance.NewSession += AudioManager_NewSession;
                }
                catch (Exception)
                {
                    r_IsAudioDeviceNotAvailable = true;
                }

            MuteToggleCommand = new DelegatedCommand(() =>
            {
                if (AudioSession != null)
                    try
                    {
                        AudioSession.IsMute = !AudioSession.IsMute;
                    }
                    catch (COMException e) when (e.ErrorCode == 0x8889004)
                    {
                        new TaskDialog()
                        {
                            Caption = StringResources.Instance.Main.Product_Name,
                            Instruction = UnhandledExceptionDialogStringResources.Instruction,
                            Icon = TaskDialogIcon.Error,
                            Content = StringResources.Instance.Main.MessageDialog_AudioSessionDisconnected,

                            OwnerWindow = App.Current.MainWindow,
                            ShowAtTheCenterOfOwner = true,
                        }.Show();
                    }
            }, () => OS.IsWin7OrLater && !r_IsAudioDeviceNotAvailable);

            SetZoomCommand = new DelegatedCommand<double>(SetZoom);
            ZoomInCommand = new DelegatedCommand(() => SetZoom(Zoom + .05));
            ZoomOutCommand = new DelegatedCommand(() => SetZoom(Zoom - .05));
            ZoomFactors = new[] { .25, .5, .75, 1.0, 1.25, 1.5, 1.75, 2.0 }.Select(r => new BrowserZoomInfo(r, SetZoomCommand)).ToList().AsReadOnly();

            RestartGameCommand = new DelegatedCommand(RestartGame);
        }

        void AudioManager_NewSession(object sender, AudioSessionCreatedEventArgs e)
        {
            if (e.Session.IsSystemSoundsSession)
                return;

            var rHostProcessID = Process.GetCurrentProcess().Id;
            int? rProcessID = e.Session.ProcessID;

            var rIsBrowserProcess = false;

            while (rProcessID.HasValue)
                using (var rManagementObject = new ManagementObject($"Win32_Process.Handle='{rProcessID.Value}'"))
                    try
                    {
                        rManagementObject.Get();
                        rProcessID = Convert.ToInt32(rManagementObject["ParentProcessId"]);

                        if (rProcessID == rHostProcessID)
                        {
                            rIsBrowserProcess = true;
                            break;
                        }
                    }
                    catch (ManagementException rException) when (rException.ErrorCode == ManagementStatus.NotFound)
                    {
                        rProcessID = null;
                    }

            if (!rIsBrowserProcess)
                return;

            e.Release = false;

            AudioSession?.Dispose();
            AudioSession = new BrowserAudioSession(e.Session);

            AudioManager.Instance.NewSession -= AudioManager_NewSession;
            AudioManager.Instance.StopSessionNotification();
        }

        void SetZoom(double rpZoom)
        {
            Preference.Instance.Browser.Zoom.Value = rpZoom;
            OnPropertyChanged(nameof(Zoom));

            foreach (var rInfo in ZoomFactors)
                rInfo.IsSelected = rInfo.Zoom == rpZoom;

            r_Owner.Communicator.Write(CommunicatorMessages.SetZoom + ":" + rpZoom);
            r_Owner.Communicator.Write(CommunicatorMessages.ResizeBrowserToFitGame);
            r_Owner.BrowserControl.Dispatcher.BeginInvoke(new Action(r_Owner.BrowserControl.ResizeBrowserToFitGame));
        }

        void RestartGame()
        {
            var rMode = Preference.Instance.Browser.RefreshConfirmationMode.Value;
            if (rMode == ConfirmationMode.Always || (rMode == ConfirmationMode.DuringSortie && KanColleGame.Current.Sortie != null))
            {
                var rDialog = new TaskDialog()
                {
                    Caption = StringResources.Instance.Main.Product_Name,
                    Instruction = StringResources.Instance.Main.Browser_RestartConfirmation_Instruction,
                    Icon = TaskDialogIcon.Information,
                    Buttons =
                    {
                        new TaskDialogCommandLink(TaskDialogCommonButton.Yes, StringResources.Instance.Main.Browser_RestartConfirmation_Button_Refresh),
                        new TaskDialogCommandLink(TaskDialogCommonButton.No, StringResources.Instance.Main.Browser_RestartConfirmation_Button_Stay),
                    },
                    DefaultCommonButton = TaskDialogCommonButton.No,

                    OwnerWindow = App.Current.MainWindow,
                    ShowAtTheCenterOfOwner = true,
                };

                if (rDialog.ShowAndDispose().ClickedCommonButton == TaskDialogCommonButton.No)
                    return;
            }

            r_Owner.Navigator.Refresh();
        }
    }
}
