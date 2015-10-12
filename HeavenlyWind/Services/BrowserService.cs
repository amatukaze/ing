using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.KanColle.Amatsukaze.Services.Browser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    class BrowserService : ModelBase
    {
        public static BrowserService Instance { get; } = new BrowserService();

        internal MemoryMappedFileCommunicator Communicator { get; private set; }
        internal IConnectableObservable<KeyValuePair<string, string>> Messages { get; private set; }

        public int HostProcessID => Process.GetCurrentProcess().Id;

        bool r_Initialized;

        Process r_BrowserProcess;

        object r_BrowserControl;
        public object BrowserControl
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

        BrowserService() { }

        public void Initialize()
        {
            if (!r_Initialized)
            {
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

                Messages.Subscribe("Ready", _ => Communicator.Write("SetPort:" + Preference.Current.Network.Port));
                Messages.SubscribeOnDispatcher("Attach", rpHandle => Attach((IntPtr)int.Parse(rpHandle)));

                r_Initialized = true;

            }
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
        }

    }
}
