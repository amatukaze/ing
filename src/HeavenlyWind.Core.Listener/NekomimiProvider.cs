using System;
using System.Collections.Generic;
using System.IO;
using Sakuno.Nekomimi;

namespace Sakuno.KanColle.Amatsukaze.Services.Listener
{
    internal class NekomimiProvider : ITextStreamProvider
    {
        public event Action<KeyValuePair<string, Stream>> Received;
        private readonly ProxyServer server;
        public NekomimiProvider(ProxyServer server)
        {
            this.server = server;
            server.AfterResponse += Server_AfterResponse;
        }

        private void Server_AfterResponse(Session session)
        {
            var response = session.GetResponseBodyStream();
            if (session.LocalPath.StartsWith("/kcsapi/"))
            {
                Received?.Invoke(new KeyValuePair<string, Stream>
                (
                    session.LocalPath.Substring(8),// /kcsapi/
                    new SkippingStream(response, 7)// svdata=
                ));
            }
        }
    }
}
