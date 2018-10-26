using System;
using System.Net;
using Sakuno.ING.Composition;
using Sakuno.ING.Http;
using Sakuno.ING.Messaging;
using Sakuno.ING.Settings;
using Sakuno.ING.Timing;
using Sakuno.Nekomimi;

namespace Sakuno.ING.Services.Listener
{
    [Export(typeof(IHttpProxy))]
    internal class NekomimiProvider : IHttpProxy
    {
        public event TimedMessageHandler<HttpMessage> Received;
        private readonly ProxyServer server = new ProxyServer();
        private readonly ITimingService dateTime;
        private readonly ProxySetting setting;

        public NekomimiProvider(ITimingService dateTime, ProxySetting setting)
        {
            this.dateTime = dateTime;
            this.setting = setting;
            server.AfterResponse += Server_AfterResponse;

            ListeningPort = setting.ListeningPort.InitialValue;

            setting.UseUpstream.PropertyChanged += UpdateUpstream;
            setting.Upstream.PropertyChanged += UpdateUpstream;
            setting.UpstreamPort.PropertyChanged += UpdateUpstream;

            UpdateUpstream(null, null);
        }

        public int ListeningPort { get; }

        private bool isEnabled;
        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                isEnabled = value;
                CheckServer();
            }
        }

        private IWebProxy proxy;
        private void UpdateUpstream(object sender, object e)
        {
            if (setting.UseUpstream.Value)
                proxy = new Proxy($"http://{setting.Upstream.Value}:{setting.UpstreamPort.Value}");
            else
                proxy = null;
            CheckServer();
        }

        private void CheckServer()
        {
            server.Stop();
            if (IsEnabled)
            {
                server.UpstreamProxy = proxy;
                server.Start(ListeningPort);
            }
        }

        private void Server_AfterResponse(Session session)
        {
            var localPath = session.Request.RequestUri.LocalPath;
            if (localPath.StartsWith("/kcsapi/"))
            {
                Received?.Invoke
                (
                    session.Response.Headers.Date ?? dateTime.Now,
                    new HttpMessage
                    (
                        localPath.Substring(8),// /kcsapi/
                        session.Request.Content.ReadAsStringAsync().Result.AsMemory(),
                        session.Response.Content.ReadAsStringAsync().Result.AsMemory(7) // svdata=
                    )
                );
            }
        }
    }
}
