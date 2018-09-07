using System;
using System.IO;
using Sakuno.ING.Http;
using Sakuno.ING.Messaging;

namespace Sakuno.ING.Game.Tests
{
    internal class UnitTestProvider : IHttpProvider
    {
        public bool Enabled { get; set; }

        public event TimedMessageHandler<HttpMessage> Received;

        public void Push(string key, DateTimeOffset timeStamp, string request, Stream stream)
            => Received?.Invoke(timeStamp, new HttpMessage(key, request, stream));
    }
}
