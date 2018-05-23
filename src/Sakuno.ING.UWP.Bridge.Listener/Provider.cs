using System;
using System.IO;
using System.Threading;
using Sakuno.ING.Messaging;
using Sakuno.ING.Services;
using Windows.Web.Http;

namespace Sakuno.ING.UWP.Bridge
{
    internal class Provider : BindableObject, ITextStreamProvider
    {
        private readonly IDateTimeService dateTimeService;
        public Provider(IDateTimeService dateTimeService) => this.dateTimeService = dateTimeService;

        private bool _connected;
        public bool IsConnected
        {
            get => _connected;
            set
            {
                if (_connected != value)
                {
                    _connected = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private bool _enable;
        public bool Enabled
        {
            get => _enable;
            set
            {
                if (_enable == value) return;
                _enable = value;

                if (value)
                {
                    cts = new CancellationTokenSource();
                    ReceiverLoop(cts.Token);
                }
                else
                {
                    cts?.Cancel();
                    cts = null;
                }
            }
        }

        public event TimedMessageHandler<TextMessage> Received;

        private CancellationTokenSource cts;
        private readonly HttpClient client = new HttpClient();
        private async void ReceiverLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    string host, path, request, response;
                    MemoryStream mms;
                    DateTimeOffset timeStamp;
                    using (var stream = (await client.GetInputStreamAsync(new Uri(Constants.HttpHost))).AsStreamForRead())
                    {
                        IsConnected = true;
                        var reader = new StreamReader(stream);
                        host = await reader.ReadLineAsync();
                        if (string.IsNullOrEmpty(host)) continue;
                        path = (await reader.ReadLineAsync()).Substring(8);
                        timeStamp = DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(await reader.ReadLineAsync()));
                        request = await reader.ReadLineAsync();
                        response = (await reader.ReadToEndAsync()).Substring(7);
                        mms = new MemoryStream();
                        var w = new StreamWriter(mms);
                        w.WriteLine(response);
                        w.Flush();
                        mms.Seek(0, SeekOrigin.Begin);
                    }
                    Received?.Invoke(timeStamp, new TextMessage(path, request, mms));
                }
                catch
                {
                    IsConnected = false;
                }
            }
        }
    }
}
