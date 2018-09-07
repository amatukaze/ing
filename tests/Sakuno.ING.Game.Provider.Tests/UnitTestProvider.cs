using System;
using System.IO;
using Sakuno.ING.Http;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game.Tests
{
    internal class UnitTestProvider : IHttpProvider
    {
        public event TimedMessageHandler<HttpMessage> Received;

        public void Push(string key, DateTimeOffset timeStamp, Stream request, Stream response)
        {
            var mms = new MemoryStream();
            mms.Write(new byte[7], 0, 7);
            response.CopyTo(mms);
            mms.Seek(0, SeekOrigin.Begin);
            Received?.Invoke(timeStamp, new HttpMessage(key, request ?? new MemoryStream(0), mms));
        }
    }

    internal class UnitTestProviderSelector : IHttpProviderSelector
    {
        public UnitTestProviderSelector(UnitTestProvider provider)
            => Current = provider;
        public IHttpProvider Current { get; }
    }
}
