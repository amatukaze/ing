using Sakuno.SystemInterop;
using System;
using System.Diagnostics;
using System.Management;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class GameController : ModelBase
    {
        BrowserService r_Owner;

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

        public ICommand TakeScreenshotToFileCommand { get; }
        public ICommand TakeScreenshotToClipboardCommand { get; }

        public ICommand MuteToggleCommand { get; }

        public ICommand RestartGameCommand { get; }

        public GameController(BrowserService rpOwner)
        {
            r_Owner = rpOwner;

            TakeScreenshotToFileCommand = new DelegatedCommand(() => ScreenshotService.Instance.TakeScreenshotAndOutput(rpOutputToClipboard: false));
            TakeScreenshotToClipboardCommand = new DelegatedCommand(() => ScreenshotService.Instance.TakeScreenshotAndOutput(rpOutputToClipboard: true));

            if (OS.IsWin7OrLater && !rpOwner.NoInstalledLayoutEngines)
                try
                {
                    foreach (var rSession in VolumeManager.Instance.EnumerateSessions())
                        rSession.Dispose();

                    VolumeManager.Instance.NewSession += VolumeManager_NewSession;
                }
                catch (Exception)
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
            if (rpSession.DisplayName.OICEquals(@"@%SystemRoot%\System32\AudioSrv.Dll,-202"))
                return;

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
            r_Owner.Navigator.Refresh();
        }

    }
}
