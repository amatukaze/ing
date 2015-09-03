using Fiddler;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using Sakuno.KanColle.Amatsukaze.Models;
using System.Reactive.Subjects;

namespace Sakuno.KanColle.Amatsukaze.Game.Proxy
{
    public static class KanColleProxy
    {
        public static Subject<NetworkSession> SessionSubject { get; } = new Subject<NetworkSession>();

        static KanColleProxy()
        {
            FiddlerApplication.BeforeRequest += FiddlerApplication_BeforeRequest;
            FiddlerApplication.OnReadResponseBuffer += FiddlerApplication_OnReadResponseBuffer;
            FiddlerApplication.ResponseHeadersAvailable += FiddlerApplication_ResponseHeadersAvailable;
            FiddlerApplication.BeforeResponse += FiddlerApplication_BeforeResponse;
            FiddlerApplication.BeforeReturningError += FiddlerApplication_BeforeReturningError;
            FiddlerApplication.AfterSessionComplete += FiddlerApplication_AfterSessionComplete;
        }

        public static void Start()
        {
            var rStartupFlags = FiddlerCoreStartupFlags.ChainToUpstreamGateway;
            if (Preference.Current.Network.EnableForSSL)
                rStartupFlags |= FiddlerCoreStartupFlags.DecryptSSL;

            FiddlerApplication.Startup(Preference.Current.Network.Port, rStartupFlags);
        }

        static void FiddlerApplication_BeforeRequest(Session rpSession)
        {
            if (Preference.Current.Network.UpstreamProxy.Enabled)
                rpSession["x-OverrideGateway"] = Preference.Current.Network.UpstreamProxy.Address;

            var rRequest = rpSession.oRequest;

            var rFullUrl = rpSession.fullUrl;
            var rPath = rpSession.PathAndQuery;

            NetworkSession rSession;
            if (!rPath.StartsWith("/kcs"))
                rSession = new NetworkSession(rFullUrl);
            else if (rPath[4] == '/')
                rSession = new ResourceSession(rFullUrl, rPath);
            else
                rSession = new ApiSession(rFullUrl);

            rSession.RequestBodyString = rpSession.GetRequestBodyAsString();

            rpSession.Tag = rSession;

            SessionSubject.OnNext(rSession);
        }

        static void FiddlerApplication_OnReadResponseBuffer(object sender, RawReadEventArgs e)
        {
            var rSession = e.sessionOwner.Tag as NetworkSession;
            if (rSession != null)
                rSession.LoadedBytes += e.iCountOfBytes;
        }
        static void FiddlerApplication_ResponseHeadersAvailable(Session rpSession)
        {
            var rSession = rpSession.Tag as NetworkSession;
            var rContentLength = rpSession.oResponse["Content-Length"];
            if (!rContentLength.IsNullOrEmpty() && rSession != null)
                rSession.ContentLength = int.Parse(rContentLength);
        }

        static void FiddlerApplication_BeforeResponse(Session rpSession)
        {
            var rSession = rpSession.Tag as NetworkSession;
            if (rSession != null)
            {
                if (rSession.State == NetworkSessionState.Requested)
                    rSession.State = NetworkSessionState.Responsed;

                var rApiSession = rSession as ApiSession;
                if (rApiSession != null)
                {
                    rApiSession.ResponseBodyString = rpSession.GetResponseBodyAsString();
                    ApiParserManager.Instance.Process(rApiSession);
                }
            }
        }

        static void FiddlerApplication_BeforeReturningError(Session rpSession)
        {
            var rSession = rpSession.Tag as NetworkSession;
            if (rSession != null)
            {
                rSession.State = NetworkSessionState.Error;
                rSession.ErrorMessage = rpSession.GetResponseBodyAsString();
            }
        }

        static void FiddlerApplication_AfterSessionComplete(Session rpSession)
        {
            var rSession = rpSession.Tag as NetworkSession;
            if (rSession != null)
                rSession.StatusCode = rpSession.responseCode;
        }
    }
}
