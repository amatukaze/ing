using System;
using System.IO;
using Sakuno.ING.Messaging;
using Sakuno.ING.Services;

namespace Sakuno.ING.Game.Tests
{
    internal class UnitTestProvider : ITextStreamProvider
    {
        public bool Enabled { get; set; }

        public event TimedMessageHandler<TextMessage> Received;

        public void Push(string key, DateTimeOffset timeStamp, string request, Stream stream)
            => Received?.Invoke(timeStamp, new TextMessage(key, request, stream));
    }
}
