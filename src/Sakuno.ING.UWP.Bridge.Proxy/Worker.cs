using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Sakuno.Nekomimi;

namespace Sakuno.ING.UWP.Bridge
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
        private BlockingCollection<(string Host, string Path, DateTimeOffset TimeStamp, string Request, string Response)> sessionCache
            = new BlockingCollection<(string, string, DateTimeOffset, string, string)>(10);
        private Task<string> emptyStringTask = Task.FromResult(string.Empty);
        public void Start()
        {
            IsListening = true;
            server.AfterResponse += session =>
            {
                if (!session.IsHTTPS && session.Request.RequestUri.LocalPath.StartsWith("/kcsapi"))
                    while (!sessionCache.TryAdd(
                        (session.Request.RequestUri.Host,
                        session.Request.RequestUri.LocalPath,
                        session.Response.Headers.Date ?? DateTimeOffset.UtcNow,
                        session.Request.Content?.ReadAsStringAsync().Result ?? string.Empty,
                        session.Response.Content?.ReadAsStringAsync().Result ?? string.Empty)))
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
                try
                {
                    var context = await sysListener.GetContextAsync();
                    using (var response = context.Response)
                    {
                        IsConnected = true;
                        using (var stream = response.OutputStream)
                        {
                            var writer = new StreamWriter(stream);
                            var session = sessionCache.Take();
                            await writer.WriteLineAsync(session.Host);
                            await writer.WriteLineAsync(session.Path);
                            await writer.WriteLineAsync(session.TimeStamp.ToUnixTimeMilliseconds().ToString());
                            await writer.WriteLineAsync(session.Request);
                            await writer.WriteLineAsync(session.Response);
                            await writer.FlushAsync();
                        }
                    }
                }
                catch
                {
                    IsConnected = false;
                }
            }
        }
    }
}
