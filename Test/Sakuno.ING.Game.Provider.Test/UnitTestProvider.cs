using System;
using System.IO;
using Sakuno.ING.Services;

namespace Sakuno.ING.Game.Test
{
    internal class UnitTestProvider : ITextStreamProvider
    {
        public bool Enabled { get; set; }

        public event Action<TextMessage> Received;

        public void Push(string key, DateTimeOffset timeStamp, string request, Stream stream)
            => Received?.Invoke(new TextMessage(key, timeStamp, request, stream));
    }
}
