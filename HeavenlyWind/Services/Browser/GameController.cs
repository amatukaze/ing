using Sakuno.KanColle.Amatsukaze.Game;
using Sakuno.KanColle.Amatsukaze.Game.Models;
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
                    AudioManager.StartSessionNotification();
                    AudioManager.NewSession += AudioManager_NewSession;
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
                    catch (COMException e) when (e.ErrorCode != 0x8889004)
                    {
                        new TaskDialog()
                        {
                            Caption = StringResources.Instance.Main.Product_Name,
                            Instruction = UnhandledExceptionDialogStringResources.Instruction,
                            Icon = TaskDialogIcon.Error,
                            Content = e.ErrorCode.ToString(),

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

        void AudioManager_NewSession(AudioSessionCreatedEventArgs e)
        {
            if (e.Session.IsSystemSoundsSession)
                return;

            var rHostProcessID = Process.GetCurrentProcess().Id;
            var rProcessID = e.Session.ProcessID;

            var rSnapshot = NativeMethods.Kernel32.CreateToolhelp32Snapshot(NativeEnums.TH32CS.TH32CS_SNAPPROCESS, 0);
            var rEntry = new NativeStructs.PROCESSENTRY32() { dwSize = Marshal.SizeOf(typeof(NativeStructs.PROCESSENTRY32)) };

            var rIsCurrentProcess = false;
            var rIsMatch = false;

            if (NativeMethods.Kernel32.Process32First(rSnapshot, ref rEntry))
                do
                {
                    if (rEntry.th32ParentProcessID == rHostProcessID)
                        rIsCurrentProcess = true;

                    if (rEntry.th32ProcessID == rProcessID)
                        rIsMatch = true;
                } while (!rIsMatch && NativeMethods.Kernel32.Process32Next(rSnapshot, ref rEntry));

            NativeMethods.Kernel32.CloseHandle(rSnapshot);

            if (!rIsCurrentProcess || !rIsMatch)
                return;

            e.Release = false;

            AudioSession?.Dispose();
            AudioSession = new BrowserAudioSession(e.Session);

            AudioManager.NewSession -= AudioManager_NewSession;
            AudioManager.StopSessionNotification();
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
            var rMode = Preference.Instance.UI.CloseConfirmationMode.Value;
            if (rMode == ConfirmationMode.Always || (rMode == ConfirmationMode.DuringSortie && KanColleGame.Current.Sortie is SortieInfo && !(KanColleGame.Current.Sortie is PracticeInfo)))
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
