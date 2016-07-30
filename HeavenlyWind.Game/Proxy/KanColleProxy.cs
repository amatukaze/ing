using Fiddler;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models;
using System;
using System.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;

namespace Sakuno.KanColle.Amatsukaze.Game.Proxy
{
    public static class KanColleProxy
    {
        public static Subject<NetworkSession> SessionSubject { get; } = new Subject<NetworkSession>();

        static Regex r_FlashQualityRegex = new Regex("(\"quality\"\\s+:\\s+\")\\w+(\",)", RegexOptions.Multiline);
        static Regex r_FlashRenderModeRegex = new Regex("(\"wmode\"\\s+:\\s+\")\\w+(\",)", RegexOptions.Multiline);

        static Regex r_SuppressReloadConfirmation = new Regex("(?<=if \\()confirm\\(\"エラーが発生したため、ページ更新します。\"\\)(?=\\) {)");

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
            if (Preference.Instance.Network.AllowRequestsFromOtherDevices)
                rStartupFlags |= FiddlerCoreStartupFlags.AllowRemoteClients;

            FiddlerApplication.Startup(Preference.Instance.Network.Port, rStartupFlags);
        }

        static void FiddlerApplication_BeforeRequest(Session rpSession)
        {
            var rUpstreamProxyPreference = Preference.Instance.Network.UpstreamProxy;
            if (rUpstreamProxyPreference.Enabled && (!rUpstreamProxyPreference.HttpOnly || !rpSession.RequestMethod.OICEquals("CONNECT")))
                rpSession["x-OverrideGateway"] = $"{rUpstreamProxyPreference.Host.Value}:{rUpstreamProxyPreference.Port.Value}";

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
            rSession.Method = rpSession.RequestMethod;

            rpSession.Tag = rSession;

            if (rFullUrl == GameConstants.GamePageUrl || rPath == "/gadget/js/kcs_flash.js")
                rpSession.bBufferResponse = true;

            var rResourceSession = rSession as ResourceSession;
            if (rResourceSession != null)
                CacheService.Instance.ProcessRequest(rResourceSession, rpSession);

            rSession.RequestHeaders = rpSession.RequestHeaders.Select(r => new SessionHeader(r.Name, r.Value)).ToArray();

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
                    rSession.ResponseBodyString = rpSession.GetResponseBodyAsString();
                    ApiParserManager.Instance.Process(rApiSession);
                }

                var rResourceSession = rSession as ResourceSession;
                if (rResourceSession != null)
                    CacheService.Instance.ProcessResponse(rResourceSession, rpSession);

                if (rpSession.PathAndQuery == "/gadget/js/kcs_flash.js")
                {
                    var rScript = rpSession.GetResponseBodyAsString();
                    var rModified = false;

                    var rQuality = Preference.Instance.Browser.Flash.Quality;
                    if (rQuality != FlashQuality.Default)
                    {
                        rScript = r_FlashQualityRegex.Replace(rScript, $"$1{rQuality}$2");
                        rModified = true;
                    }

                    var rRenderMode = Preference.Instance.Browser.Flash.RenderMode;
                    if (rRenderMode != FlashRenderMode.Default)
                    {
                        rScript = r_FlashRenderModeRegex.Replace(rScript, $"$1{rRenderMode}$2");
                        rModified = true;
                    }

                    if (rModified)
                        rpSession.utilSetResponseBody(rScript);
                }

                if (rSession.FullUrl == GameConstants.GamePageUrl)
                {
                    ForceOverrideStylesheet(rpSession);

                    var rSource = rpSession.GetResponseBodyAsString();
                    rSource = r_SuppressReloadConfirmation.Replace(rSource, "false");

                    rpSession.utilSetResponseBody(rSource);
                }

                rSession.StatusCode = rpSession.responseCode;
                rSession.ResponseHeaders = rpSession.ResponseHeaders.Select(r => new SessionHeader(r.Name, r.Value)).ToArray();
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

            var rResourceSession = rSession as ResourceSession;
            if (rResourceSession != null)
                CacheService.Instance.ProcessOnCompletion(rResourceSession, rpSession);
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
    z-index: 255;
}
</style></head>");
        }

    }
}
