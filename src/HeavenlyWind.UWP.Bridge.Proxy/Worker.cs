using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Net;
using Sakuno.Nekomimi;

namespace Sakuno.KanColle.Amatsukaze.UWP.Bridge
{
    class Worker : INotifyPropertyChanged
    {
        public string Version => Constants.Version;
        public int Port { get; set; } = 15551;
        public string Upstream { get; set; } = "127.0.0.1";
        public int UpstreamPort { get; set; } = 8099;
        public bool UseUpstream { get; set; }

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

        private string _error;
        public string Error
        {
            get => _error;
            private set
            {
                _error = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Error)));
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
                if (!session.IsHTTPS && session.Request.RequestUri.LocalPath.StartsWith("/kcsapi"))
                    while (!sessionCache.TryAdd(session))
                    {
                        IsConnected = false;
                        sessionCache.TryTake(out _, TimeSpan.FromMilliseconds(100));
                    }
            };
            server.SessionFailed += (session, e) => Error = e.ToString();
            sysListener.Prefixes.Add(Constants.HttpHost);
            sysListener.Start();
            RunReverseHandler();
        }

        public void Update()
        {
            IsListening = false;
            if (UseUpstream)
                server.UpstreamProxy = new Proxy($"http://{Upstream}:{UpstreamPort}");
            else
                server.UpstreamProxy = null;
            IsListening = true;
        }

        private async void RunReverseHandler()
        {
            while (sysListener.IsListening)
            {
                var context = await sysListener.GetContextAsync();
                using (var response = context.Response)
                {
                    IsConnected = true;
                    response.SendChunked = true;
                    using (var stream = response.OutputStream)
                    {
                        var writer = new StreamWriter(stream);
                        Session session = null;
                        try
                        {
                            session = sessionCache.Take();
                        }
                        catch (Exception e)
                        {
                            System.Windows.MessageBox.Show(e.Message);
                        }

                        if (session == null)
                        {
                            await writer.WriteLineAsync();
                        }
                        else
                        {
                            await writer.WriteLineAsync(session.Request.RequestUri.Host);
                            await writer.WriteLineAsync(session.Request.RequestUri.LocalPath);
                            await writer.WriteLineAsync(session.Response.Headers.Date?.ToUnixTimeMilliseconds().ToString());
                            await writer.WriteLineAsync(session.Request.Content != null ?
                                await session.Request.Content.ReadAsStringAsync() : string.Empty);
                            await (await session.Response.Content.ReadAsStreamAsync()).CopyToAsync(stream);
                        }
                    }
                }
            }
        }
    }
}
