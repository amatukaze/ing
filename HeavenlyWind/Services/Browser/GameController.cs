using Sakuno.SystemInterop;
using System;
using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class GameController : ModelBase
    {
        bool r_IsAudioDeviceNotAvailable;
        BrowserVolume r_Volume;
        public BrowserVolume Volume
        {
            get { return r_Volume; }
            private set
            {
                if (r_Volume != value)
                {
                    r_Volume = value;
                    OnPropertyChanged(nameof(Volume));
                }
            }
        }

        public ICommand TakeScreenshotCommand { get; }
        public ICommand MuteToggleCommand { get; }

        public ICommand RestartGameCommand { get; }

        public GameController()
        {
            TakeScreenshotCommand = new DelegatedCommand(() => ScreenshotService.Instance.TakeScreenshotAndOutput(rpOutputToClipboard: false));

            if (OS.IsWin7OrLater)
                try
                {
                    foreach (var rSession in VolumeManager.Instance.EnumerateSessions())
                        rSession.Dispose();

                    VolumeManager.Instance.NewSession += VolumeManager_NewSession;
                }
                catch (TypeInitializationException e) when (e.InnerException is COMException)
                {
                    r_IsAudioDeviceNotAvailable = true;
                }

            MuteToggleCommand = new DelegatedCommand(() =>
            {
                if (Volume != null)
                    Volume.IsMute = !Volume.IsMute;
            }, () => OS.IsWin7OrLater && !r_IsAudioDeviceNotAvailable);

            RestartGameCommand = new DelegatedCommand(RestartGame);
        }

        void VolumeManager_NewSession(VolumeSession rpSession)
        {
            var rHostProcessID = Process.GetCurrentProcess().Id;
            int? rProcessID = rpSession.ProcessID;

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
                    catch (ManagementException e) when (e.ErrorCode == ManagementStatus.NotFound)
                    {
                        rProcessID = null;
                    }

            if (!rIsBrowserProcess)
                return;

            Volume?.Dispose();
            Volume = new BrowserVolume(rpSession);

            VolumeManager.Instance.NewSession -= VolumeManager_NewSession;
        }

        void RestartGame()
        {
            BrowserService.Instance.Navigator.Refresh();
        }

    }
}
