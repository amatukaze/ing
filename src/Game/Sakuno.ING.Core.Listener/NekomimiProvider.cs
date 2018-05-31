using System;
using Sakuno.ING.Messaging;
using Sakuno.ING.Settings;
using Sakuno.ING.Timing;
using Sakuno.Nekomimi;

namespace Sakuno.ING.Services.Listener
{
    internal class NekomimiProvider : ITextStreamProvider
    {
        public event TimedMessageHandler<TextMessage> Received;
        private readonly ProxyServer server;
        private readonly ITimingService dateTime;
        private readonly ProxySetting setting;
        private bool enabled;
        public bool Enabled
        {
            get => enabled;
            set
            {
                if (enabled != value)
                {
                    enabled = value;
                    if (value)
                        server.Start(setting.ListeningPort.Value);
                    else
                        server.Stop();
                }
            }
        }

        public NekomimiProvider(ProxyServer server, ITimingService dateTime, ProxySetting setting)
        {
            this.server = server;
            this.dateTime = dateTime;
            this.setting = setting;
            server.AfterResponse += Server_AfterResponse;

            setting.ListeningPort.PropertyChanged += (_, __) =>
            {
                if (Enabled)
                {
                    Enabled = false;
                    Enabled = true;
                }
            };

            setting.UseUpstream.PropertyChanged += UpdateUpstream;
            setting.Upstream.PropertyChanged += UpdateUpstream;
            setting.UpstreamPort.PropertyChanged += UpdateUpstream;

            UpdateUpstream(null, null);
        }

        private void UpdateUpstream(object sender, object e)
        {
            server.Stop();
            if (setting.UseUpstream.Value)
                server.UpstreamProxy = new Proxy($"http://{setting.Upstream.Value}:{setting.UpstreamPort.Value}");
            else
                server.UpstreamProxy = null;
            server.Start(setting.ListeningPort.Value);
        }

        private void Server_AfterResponse(Session session)
        {
            var localPath = session.Request.RequestUri.LocalPath;
            if (localPath.StartsWith("/kcsapi/"))
            {
                Received?.Invoke
                (
                    session.Response.Headers.Date ?? DateTimeOffset.UtcNow,
                    new TextMessage
                    (
                        localPath.Substring(8),// /kcsapi/
                        session.Request.Content.ReadAsStringAsync().Result,
                        new SkippingStream(session.Response.Content.ReadAsStreamAsync().Result, 7)// svdata=
                    )
                );
            }
        }
    }
}
