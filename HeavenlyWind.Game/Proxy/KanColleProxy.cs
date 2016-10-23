using Fiddler;
using Sakuno.KanColle.Amatsukaze.Extensibility;
using Sakuno.KanColle.Amatsukaze.Extensibility.Services;
using Sakuno.KanColle.Amatsukaze.Game.Parsers;
using Sakuno.KanColle.Amatsukaze.Game.Services;
using Sakuno.KanColle.Amatsukaze.Models;
using Sakuno.SystemInterop.Net;
using System;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reactive.Subjects;
using System.Text.RegularExpressions;
using System.Threading;

namespace Sakuno.KanColle.Amatsukaze.Game.Proxy
{
    public static class KanColleProxy
    {
        public static Subject<NetworkSession> SessionSubject { get; } = new Subject<NetworkSession>();

        static Regex r_RemoveGoogleAnalyticsRegex = new Regex(@"gapush\(.+?\);", RegexOptions.Singleline);
        static Regex r_UserIDRegex { get; } = new Regex(@"(?:(?<=api_world%2Fget_id%2F)|(?<=api_world\\/get_id\\/)|(?<=api_auth_member\\/dmmlogin\\/))\d+");
        static Regex r_TokenResponseRegex { get; } = new Regex(@"(?<=\\""api_token\\"":\\"")\w+");

        static Regex r_FlashQualityRegex = new Regex("(\"quality\"\\s+:\\s+\")\\w+(\",)", RegexOptions.Singleline);
        static Regex r_FlashRenderModeRegex = new Regex("(\"wmode\"\\s+:\\s+\")\\w+(\",)", RegexOptions.Singleline);

        static Regex r_SuppressReloadConfirmation = new Regex("(?<=if \\()confirm\\(\"エラーが発生したため、ページ更新します。\"\\)(?=\\) {)");

        static string[] r_BlockingList;

        static string r_UpstreamProxy;

        static ManualResetEventSlim r_TrafficBarrier;

        static SQLiteConnection r_Connection;

        static KanColleProxy()
        {
            FiddlerApplication.BeforeRequest += FiddlerApplication_BeforeRequest;
            FiddlerApplication.OnReadResponseBuffer += FiddlerApplication_OnReadResponseBuffer;
            FiddlerApplication.ResponseHeadersAvailable += FiddlerApplication_ResponseHeadersAvailable;
            FiddlerApplication.BeforeResponse += FiddlerApplication_BeforeResponse;
            FiddlerApplication.BeforeReturningError += FiddlerApplication_BeforeReturningError;
            FiddlerApplication.AfterSessionComplete += FiddlerApplication_AfterSessionComplete;

            if (File.Exists(@"Data\BlockingList.lst"))
                r_BlockingList = File.ReadAllLines(@"Data\BlockingList.lst");
            else
                r_BlockingList = ArrayUtil.Empty<string>();

            Preference.Instance.Network.UpstreamProxy.Enabled.Subscribe(_ => UpdateUpstreamProxy());
            Preference.Instance.Network.UpstreamProxy.Host.Subscribe(_ => UpdateUpstreamProxy());
            Preference.Instance.Network.UpstreamProxy.Port.Subscribe(_ => UpdateUpstreamProxy());
            UpdateUpstreamProxy();

            InitializeAntiBlankScreenDataCollector();
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
            if (r_BlockingList.Any(rpSession.uriContains))
            {
                rpSession.utilCreateResponseAndBypassServer();
                return;
            }

            if (!r_UpstreamProxy.IsNullOrEmpty())
                rpSession["x-OverrideGateway"] = r_UpstreamProxy;

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

            if (rFullUrl.OICEquals(GameConstants.GamePageUrl) || rFullUrl.OICEquals("http://www.dmm.com/netgame_s/kancolle/") || rPath.OICEquals("/gadget/js/kcs_flash.js"))
                rpSession.bBufferResponse = true;

            var rResourceSession = rSession as ResourceSession;
            if (rResourceSession != null)
                CacheService.Instance.ProcessRequest(rResourceSession, rpSession);

            rSession.RequestHeaders = rpSession.RequestHeaders.Select(r => new SessionHeader(r.Name, r.Value)).ToArray();

            SessionSubject.OnNext(rSession);

            if (!rpSession.bHasResponse && r_TrafficBarrier != null)
                r_TrafficBarrier.Wait();
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

                if (rSession.FullUrl.OICStartsWith("http://osapi.dmm.com/gadgets/makeRequest"))
                    using (var rCommand = r_Connection.CreateCommand())
                    {
                        rCommand.CommandText = "INSERT INTO anti_blank_screen.history(time, url, body) VALUES(@time, @url, @body);";
                        rCommand.Parameters.AddWithValue("@time", DateTimeOffset.Now.Ticks);
                        rCommand.Parameters.AddWithValue("@url", r_UserIDRegex.Replace(rSession.FullUrl, "******"));

                        var rBody = rpSession.GetResponseBodyAsString();
                        rBody = r_UserIDRegex.Replace(rBody, "******");
                        rBody = r_TokenResponseRegex.Replace(rBody, "******");
                        rCommand.Parameters.AddWithValue("@body", r_UserIDRegex.Replace(rBody, "******"));

                        rCommand.ExecuteNonQuery();
                    }

                var rApiSession = rSession as ApiSession;
                if (rApiSession != null)
                {
                    rSession.ResponseBodyString = rpSession.GetResponseBodyAsString();
                    ApiParserManager.Process(rApiSession);
                }

                var rResourceSession = rSession as ResourceSession;
                if (rResourceSession != null)
                    CacheService.Instance.ProcessResponse(rResourceSession, rpSession);

                if (rpSession.PathAndQuery.OICEquals("/gadget/js/kcs_flash.js"))
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

                if (rSession.FullUrl.OICEquals("http://www.dmm.com/netgame_s/kancolle/"))
                {
                    var rSource = rpSession.GetResponseBodyAsString();
                    rSource = r_RemoveGoogleAnalyticsRegex.Replace(rSource, string.Empty);

                    rpSession.utilSetResponseBody(rSource);
                }

                if (rSession.FullUrl.OICEquals(GameConstants.GamePageUrl))
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

        static void UpdateUpstreamProxy()
        {
            var rUpstreamProxyPreference = Preference.Instance.Network.UpstreamProxy;
            if (rUpstreamProxyPreference.Enabled)
                r_UpstreamProxy = rUpstreamProxyPreference.Host.Value + ":" + rUpstreamProxyPreference.Port.Value;
            else
                r_UpstreamProxy = null;
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

        static void InitializeTrafficBarrier()
        {
            try
            {
                r_TrafficBarrier = new ManualResetEventSlim(NetworkListManager.IsConnectedToInternet);
                NetworkListManager.ConnectivityChanged += delegate
                {
                    if (NetworkListManager.IsConnectedToInternet)
                        r_TrafficBarrier.Set();
                    else
                        r_TrafficBarrier.Reset();
                };
            }
            catch
            {
            }

            ServiceManager.Register<INetworkAvailabilityService>(new NetworkAvailabilityService());
        }

        static void InitializeAntiBlankScreenDataCollector()
        {
            using (var rConnection = new SQLiteConnection(@"Data Source=Data\AntiBlankScreen.db; Page Size=8192").OpenAndReturn())
            using (var rCommand = rConnection.CreateCommand())
            {
                rCommand.CommandText = "CREATE TABLE IF NOT EXISTS history(" +
                    "time INTEGER PRIMARY KEY NOT NULL, " +
                    "url TEXT NULL, " +
                    "body TEXT NULL);";

                rCommand.ExecuteNonQuery();
            }

            r_Connection = CoreDatabase.Connection;
            using (var rCommand = r_Connection.CreateCommand())
            {
                rCommand.CommandText = "ATTACH @filename AS anti_blank_screen;";
                rCommand.Parameters.AddWithValue("@filename", new FileInfo(@"Data\AntiBlankScreen.db").FullName);

                rCommand.ExecuteNonQuery();
            }
        }

        class NetworkAvailabilityService : INetworkAvailabilityService
        {
            public void EnsureNetwork() => r_TrafficBarrier?.Wait();
        }
    }
}
