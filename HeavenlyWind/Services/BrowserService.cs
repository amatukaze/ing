using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sakuno.KanColle.Amatsukaze.Services.Browser;
using Sakuno.UserInterface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    class BrowserService : ModelBase
    {
        public static BrowserService Instance { get; } = new BrowserService();

        internal MemoryMappedFileCommunicator Communicator { get; private set; }
        internal IConnectableObservable<KeyValuePair<string, string>> Messages { get; private set; }

        public IReadOnlyCollection<LayoutEngineInfo> InstalledLayoutEngines { get; private set; }

        public int HostProcessID => Process.GetCurrentProcess().Id;

        bool r_Initialized;

        bool r_NoInstalledLayoutEngines;
        public bool NoInstalledLayoutEngines
        {
            get { return r_NoInstalledLayoutEngines; }
            private set
            {
                if (r_NoInstalledLayoutEngines != value)
                {
                    r_NoInstalledLayoutEngines = value;
                    OnPropertyChanged(nameof(NoInstalledLayoutEngines));
                }
            }
        }

        Process r_BrowserProcess;

        BrowserHost r_BrowserControl;
        public BrowserHost BrowserControl
        {
            get { return r_BrowserControl; }
            private set
            {
                if (r_BrowserControl != value)
                {
                    r_BrowserControl = value;
                    OnPropertyChanged();
                }
            }
        }

        public BrowserNavigator Navigator { get; private set; }

        bool r_IsNavigatorVisible;
        public bool IsNavigatorVisible
        {
            get { return r_IsNavigatorVisible; }
            private set
            {
                if (r_IsNavigatorVisible != value)
                {
                    r_IsNavigatorVisible = value;
                    OnPropertyChanged(nameof(IsNavigatorVisible));
                }
            }
        }

        public GameController GameController { get; private set; }

        public ICommand ClearCacheCommand { get; }
        public ICommand ClearCacheAndCookieCommand { get; }

        BrowserService()
        {
            r_IsNavigatorVisible = true;

            ClearCacheCommand = new DelegatedCommand(() => Communicator.Write(CommunicatorMessages.ClearCache));
            ClearCacheAndCookieCommand = new DelegatedCommand(() => Communicator.Write(CommunicatorMessages.ClearCacheAndCookie));
        }

        public void Initialize()
        {
            if (!r_Initialized)
            {
                if (!LoadLayoutEngines())
                {
                    NoInstalledLayoutEngines = true;
                    r_Initialized = true;
                    return;
                }

                InitializeCommunicator();

                var rStartInfo = new ProcessStartInfo()
                {
                    FileName = typeof(BrowserService).Assembly.Location,
                    Arguments = $"browser {Preference.Current.Browser.CurrentLayoutEngine} {HostProcessID}",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                };
                r_BrowserProcess = Process.Start(rStartInfo);
                r_BrowserProcess.BeginOutputReadLine();
                r_BrowserProcess.OutputDataReceived += (s, e) => Trace.WriteLine(e.Data);

                Messages.Subscribe(CommunicatorMessages.Ready, _ => Communicator.Write(CommunicatorMessages.SetPort + ":" + Preference.Current.Network.Port));
                Messages.SubscribeOnDispatcher(CommunicatorMessages.Attach, rpHandle => Attach((IntPtr)int.Parse(rpHandle)));

                r_Initialized = true;

                Messages.Subscribe(CommunicatorMessages.LoadCompleted, delegate
                {
                    var rZoom = DpiUtil.ScaleX + Preference.Current.Browser.Zoom - 1.0;
                    Communicator.Write(CommunicatorMessages.SetZoom + ":" + rZoom);
                });
                Messages.Subscribe(CommunicatorMessages.LoadGamePageCompleted, _ => ResizeBrowserToFitGame());

                Navigator = new BrowserNavigator(this);
                GameController = new GameController(this);
            }
        }
        bool LoadLayoutEngines()
        {
            var rBrowsersDirectory = new DirectoryInfo(Path.Combine(Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "Browsers"));
            if (!rBrowsersDirectory.Exists)
                return false;

            try
            {
                var rInstalledLayoutEngines = rBrowsersDirectory.EnumerateFiles("*.json").Select(r =>
                {
                    using (var rReader = new JsonTextReader(File.OpenText(r.FullName)))
                        return JObject.Load(rReader).ToObject<LayoutEngineInfo>();
                });
                InstalledLayoutEngines = new ReadOnlyCollection<LayoutEngineInfo>(rInstalledLayoutEngines.ToList());

                if (InstalledLayoutEngines.Count == 0)
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }
        void InitializeCommunicator()
        {
            Communicator = new MemoryMappedFileCommunicator($"Sakuno/HeavenlyWind({HostProcessID})", 4096);
            Communicator.ReadPosition = 2048;
            Communicator.WritePosition = 0;

            Messages = Communicator.GetMessageObservable().Publish();
            Messages.Connect();

            Communicator.StartReader();
        }

        void Attach(IntPtr rpHandle)
        {
            BrowserControl = new BrowserHost(rpHandle);

            Navigator.Navigate(Preference.Current.Browser.Homepage);
        }

        internal void ResizeBrowserToFitGame()
        {
            if (BrowserControl == null)
                return;

            Communicator.Write(CommunicatorMessages.ResizeBrowserToFitGame);
            IsNavigatorVisible = false;

            BrowserControl.Dispatcher.BeginInvoke(new Action(BrowserControl.ResizeBrowserToFitGame));
        }
    }
}
