using System;
using Sakuno.Nekomimi;

namespace Sakuno.KanColle.Amatsukaze.Services.Listener
{
    internal class NekomimiProvider : ITextStreamProvider
    {
        public event Action<TextMessage> Received;
        private readonly ProxyServer server;
        private readonly IDateTimeService dateTime;

        public NekomimiProvider(ProxyServer server, IDateTimeService dateTime)
        {
            this.server = server;
            this.dateTime = dateTime;
            server.AfterResponse += Server_AfterResponse;
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
