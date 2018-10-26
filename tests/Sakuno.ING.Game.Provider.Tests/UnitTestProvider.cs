using System;
using System.IO;
using Sakuno.ING.Http;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game.Tests
{
    internal class UnitTestProvider : IHttpProvider
    {
        public event TimedMessageHandler<HttpMessage> Received;

        public void Push(string key, DateTimeOffset timeStamp, ReadOnlyMemory<char> request, Stream response)
            => Received?.Invoke(timeStamp, new HttpMessage(key, request, new StreamReader(response).ReadToEnd().AsMemory()));
    }

    internal class UnitTestProviderSelector : IHttpProviderSelector
    {
        public UnitTestProviderSelector(UnitTestProvider provider)
            => Current = provider;
        public IHttpProvider Current { get; }
    }
}
