using System;
using Sakuno.KanColle.Amatsukaze.Settings;
using Sakuno.Nekomimi;

namespace Sakuno.KanColle.Amatsukaze.Services.Listener
{
    internal class NekomimiProvider : ITextStreamProvider
    {
        public event Action<TextMessage> Received;
        private readonly ProxyServer server;
        private readonly IDateTimeService dateTime;
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

        public NekomimiProvider(ProxyServer server, IDateTimeService dateTime, ProxySetting setting)
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
            if (setting.UseUpstream.Value)
                server.UpstreamProxy = new Proxy
                (
                    setting.Upstream.Value,
                    setting.UpstreamPort.Value
                );
            else
                server.UpstreamProxy = null;
        }

        private void Server_AfterResponse(Session session)
        {
            var response = session.GetResponseBodyStream();
            if (session.LocalPath.StartsWith("/kcsapi/"))
            {
                Received?.Invoke(new TextMessage
                (
                    session.LocalPath.Substring(8),// /kcsapi/
                    dateTime.Now,
                    new SkippingStream(response, 7)// svdata=
                ));
            }
        }
    }
}
