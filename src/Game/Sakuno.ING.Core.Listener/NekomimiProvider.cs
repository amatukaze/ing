using System;
using Sakuno.ING.Http;
using Sakuno.ING.Messaging;
using Sakuno.ING.Settings;
using Sakuno.ING.Timing;
using Sakuno.Nekomimi;

namespace Sakuno.ING.Services.Listener
{
    public class NekomimiProvider : IHttpProvider
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

            setting.ListeningPort.ValueChanged += port =>
            {
                server.Stop();
                server.Start(port);
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
                    new HttpMessage
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
