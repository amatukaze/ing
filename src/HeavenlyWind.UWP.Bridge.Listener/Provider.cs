using System;
using System.IO;
using System.Threading;
using Sakuno.KanColle.Amatsukaze.Services;
using Windows.Web.Http;

namespace Sakuno.KanColle.Amatsukaze.UWP.Bridge
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

        public event Action<TextMessage> Received;

        private CancellationTokenSource cts;
        private readonly HttpClient client = new HttpClient();
        private async void ReceiverLoop(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    string host, path, request;
                    MemoryStream mms;
                    using (var stream = (await client.GetInputStreamAsync(new Uri(Constants.HttpHost))).AsStreamForRead())
                    {
                        IsConnected = true;
                        var reader = new StreamReader(stream);
                        host = await reader.ReadLineAsync();
                        if (string.IsNullOrEmpty(host)) continue;
                        path = await reader.ReadLineAsync();
                        request = await reader.ReadLineAsync();
                        var svdataBuffer = new byte[7];
                        await stream.ReadAsync(svdataBuffer, 0, 7);
                        mms = new MemoryStream();
                        await stream.CopyToAsync(mms);
                    }
                    Received?.Invoke(new TextMessage(path, dateTimeService.Now, mms));
                }
                catch
                {
                    IsConnected = false;
                }
            }
        }
    }
}
