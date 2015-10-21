using Sakuno.SystemInterop;
using System;
using System.Diagnostics;
using System.Management;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.Services.Browser
{
    class GameController : ModelBase
    {
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

        public ICommand RestartGameCommand { get; }

        public GameController()
        {
            if (OS.IsWin7OrLater)
            {
                foreach (var rSession in VolumeManager.Instance.EnumerateSessions())
                    rSession.Dispose();

                VolumeManager.Instance.NewSession += r =>
                {
                    var rHostProcessID = Process.GetCurrentProcess().Id;
                    int? rProcessID = r.ProcessID;

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
                    Volume = new BrowserVolume(r);
                };

            }

            RestartGameCommand = new DelegatedCommand(RestartGame);
        }

        void RestartGame()
        {
            BrowserService.Instance.Navigator.Refresh();
        }

    }
}
