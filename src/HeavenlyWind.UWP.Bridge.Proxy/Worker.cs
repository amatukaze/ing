using System.ComponentModel;
using System.Net;
using Sakuno.Nekomimi;

namespace Sakuno.KanColle.Amatsukaze.UWP.Bridge
{
    class Worker : INotifyPropertyChanged
    {
        public string Version => Constants.Version;
        private int _port = 15551;
        public int Port
        {
            get => _port;
            set
            {
                IsListening = false;
                _port = value;
                IsListening = true;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private bool _listening;
        public bool IsListening
        {
            get => _listening;
            private set
            {
                if (_listening == value) return;

                _listening = value;
                if (value)
                    server.Start(Port);
                else
                    server.Stop();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsListening)));
            }
        }
        public bool IsConnected { get; private set; }

        private ProxyServer server = new ProxyServer();
        private HttpListener sysListener = new HttpListener();
        public void Start()
        {
            IsListening = true;
            sysListener.Prefixes.Add(Constants.HttpHost);
            sysListener.Start();
            RunReverseHandler();
        }

        private async void RunReverseHandler()
        {
            while (sysListener.IsListening)
            {
                var context = await sysListener.GetContextAsync();
                using (var response = context.Response)
                {
                    response.SendChunked = true;
                    using (var stream = response.OutputStream)
                    {

                    }
                }
            }
        }
    }
}
