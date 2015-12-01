using Fiddler;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using Sakuno.KanColle.Amatsukaze.Models;
using System;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;

namespace Sakuno.KanColle.Amatsukaze.Game.Proxy
{
    public static class KanColleProxy
    {
        public static Subject<NetworkSession> SessionSubject { get; } = new Subject<NetworkSession>();

        static Regex r_FlashQualityRegex = new Regex("(\"quality\"\\s+:\\s+\")\\w+(\",)", RegexOptions.Multiline);
        static Regex r_FlashRenderModeRegex = new Regex("(\"wmode\"\\s+:\\s+\")\\w+(\",)", RegexOptions.Multiline);

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
            if (rPath.StartsWith("/kcsapi/"))
                rSession = new ApiSession(rFullUrl);
            else if (rPath.StartsWith("/kcs/") || rPath.StartsWith("/gadget/"))
                rSession = new ResourceSession(rFullUrl, rPath);
            else
                rSession = new NetworkSession(rFullUrl);

            rSession.RequestBodyString = Uri.UnescapeDataString(rpSession.GetRequestBodyAsString());

            rpSession.Tag = rSession;

            SessionSubject.OnNext(rSession);

            if (rFullUrl == GameConstants.GamePageUrl || rPath == " / gadget/js/kcs_flash.js")
                rpSession.bBufferResponse = true;
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

                if (rpSession.PathAndQuery == "/gadget/js/kcs_flash.js")
                {
                    var rScript = rpSession.GetResponseBodyAsString();
                    var rModified = false;

                    var rQuality = Preference.Current.Browser.Flash.Quality;
                    if (rQuality != FlashQuality.Default)
                    {
                        rScript = r_FlashQualityRegex.Replace(rScript, $"$1{rQuality}$2");
                        rModified = true;
                    }

                    var rRenderMode = Preference.Current.Browser.Flash.RenderMode;
                    if (rRenderMode != FlashRenderMode.Default)
                    {
                        rScript = r_FlashRenderModeRegex.Replace(rScript, $"$1{rRenderMode}$2");
                        rModified = true;
                    }

                    if (rModified)
                        rpSession.utilSetResponseBody(rScript);
                }

                if (rSession.FullUrl == GameConstants.GamePageUrl)
                    ForceOverrideStylesheet(rpSession);

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

        static void ForceOverrideStylesheet(Session rpSession)
        {
            rpSession.utilDecodeResponse();
            rpSession.utilReplaceInResponse("</head>", @"<style type=""text/css"">
html { touch-action: none }

body {
    margin: 0;
    overflow: hidden;
}

#ntg-recommend, #dmm-ntgnavi-renew { display: none !important; }

#game_frame {
    position: fixed;
    left: 50%;
    top: -16px;
    margin-left: -450px;
    z-index: 1;
}
</style></head>");
        }

    }
}
