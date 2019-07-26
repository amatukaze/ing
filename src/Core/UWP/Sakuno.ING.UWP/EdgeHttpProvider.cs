using System;
using System.IO;
using System.Runtime.InteropServices;
using Sakuno.ING.Http;
using Sakuno.ING.Messaging;
using Sakuno.ING.Settings;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;
using Windows.Web.Http;
using Windows.Web.Http.Filters;

namespace Sakuno.ING.UWP
{
    internal class EdgeHttpProvider : IHttpProvider
    {
        private readonly ProxySetting proxy;

        public EdgeHttpProvider(ProxySetting proxy)
        {
            this.proxy = proxy;
            proxy.UseUpstream.ValueChanged += _ => UpdateProxy();
            proxy.Upstream.ValueChanged += _ => UpdateProxy();
            proxy.UpstreamPort.ValueChanged += _ => UpdateProxy();
            UpdateProxy();

            filter = new HttpBaseProtocolFilter();
            client = new HttpClient(filter);
        }

        private void UpdateProxy()
        {
            if (proxy.UseUpstream.Value)
            {
                if (SetSystemProxy($"{proxy.Upstream.Value}:{proxy.UpstreamPort.Value}"))
                    return;
                else
                {
                    // invalid proxy
                }
            }
            SetSystemProxy(null);
        }

        public event TimedMessageHandler<HttpMessage> Received;
        private readonly HttpBaseProtocolFilter filter;
        private readonly HttpClient client;
        private Uri referer;
        internal async void WebResourceRequested(WebView sender, WebViewWebResourceRequestedEventArgs args)
        {
            var request = args.Request;
            string localPath = request.RequestUri.LocalPath;

            if (localPath.StartsWith("/kcs2/index.php"))
                referer = request.RequestUri;
            else if (localPath.StartsWith("/kcsapi/"))
            {
                var deferral = args.GetDeferral();
                try
                {
                    string reqStr = null;
                    if (request.Content != null)
                    {
                        using var stream = new InMemoryRandomAccessStream();
                        await request.Content.WriteToStreamAsync(stream);
                        stream.Seek(0);
                        reqStr = new StreamReader(stream.AsStream()).ReadToEnd();
                        request.Content = new HttpStringContent(reqStr, UnicodeEncoding.Utf8, "application/x-www-form-urlencoded");
                    }
                    request.Headers.Referer = referer;
                    using (request.Content)
                    using (var response = await client.SendRequestAsync(request))
                    {
                        args.Response = response;
                        deferral.Complete();

                        Received?.Invoke(response.Headers.Date ?? DateTimeOffset.Now, new HttpMessage
                        (
                            localPath.Substring(8),
                            reqStr.AsMemory(),
                            (await response.Content.ReadAsStringAsync()).AsMemory(7)
                        ));
                    }
                }
                catch
                {
                    args.Response = new HttpResponseMessage(HttpStatusCode.BadGateway);
                    deferral.Complete();
                }
            }
        }

        #region P/Invoke
        private unsafe struct INTERNET_PROXY_INFO
        {
            public int dwAccessType;
            public byte* proxy;
            public byte* proxyBypass;
        }

        [DllImport("wininet.dll", SetLastError = true, EntryPoint = "InternetSetOptionW", ExactSpelling = true, CharSet = CharSet.Unicode)]
        private static extern unsafe bool InternetSetOption(IntPtr hInternet, int dwOption, void* lpBuffer, int lpdwBufferLength);
        private static unsafe bool SetSystemProxy(string strProxy)
        {
            const int INTERNET_OPTION_PROXY = 38;
            const int INTERNET_OPEN_TYPE_PROXY = 3;
            const int INTERNET_OPEN_TYPE_DIRECT = 1;

            int length = strProxy?.Length ?? 0;
            byte* proxy = stackalloc byte[length + 1];
            if (length > 0)
                fixed (char* ptrProxy = strProxy)
                    for (int i = 0; i < length; i++)
                        proxy[i] = (byte)ptrProxy[i];
            proxy[length] = 0;
            byte* bypass = stackalloc byte[] { (byte)'l', (byte)'o', (byte)'c', (byte)'a', (byte)'l', 0 };

            var struct_IPI = new INTERNET_PROXY_INFO
            {
                dwAccessType = length == 0 ? INTERNET_OPEN_TYPE_DIRECT : INTERNET_OPEN_TYPE_PROXY,
                proxy = proxy,
                proxyBypass = length == 0 ? null : bypass
            };

            return InternetSetOption(IntPtr.Zero, INTERNET_OPTION_PROXY, &struct_IPI, sizeof(INTERNET_PROXY_INFO));
        }
        #endregion
    }
}
