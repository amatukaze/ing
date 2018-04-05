using System;
using System.Collections.Concurrent;
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
        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            private set
            {
                _isConnected = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsConnected)));
            }
        }

        private ProxyServer server = new ProxyServer();
        private HttpListener sysListener = new HttpListener();
        private BlockingCollection<Session> sessionCache = new BlockingCollection<Session>(10);
        public void Start()
        {
            IsListening = true;
            server.AfterResponse += session =>
            {
                if (!session.IsHTTPS && session.LocalPath.StartsWith("/kcsapi"))
                    while (!sessionCache.TryAdd(session))
                    {
                        IsConnected = false;
                        sessionCache.TryTake(out _, TimeSpan.FromMilliseconds(100));
                    }
            };
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
