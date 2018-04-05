using System.ComponentModel;
using Sakuno.Nekomimi;

namespace Sakuno.KanColle.Amatsukaze.UWP.Bridge
{
    class Worker : INotifyPropertyChanged
    {
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
        public Worker()
        {
            IsListening = true;
        }
    }
}
