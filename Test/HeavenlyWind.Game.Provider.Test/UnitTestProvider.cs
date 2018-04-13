using System;
using System.IO;
using Sakuno.KanColle.Amatsukaze.Services;

namespace Sakuno.KanColle.Amatsukaze.Game.Test
{
    internal class UnitTestProvider : ITextStreamProvider
    {
        public bool Enabled { get; set; }

        public event Action<TextMessage> Received;

        public void Push(string key, DateTimeOffset timeStamp, string request, Stream stream)
            => Received?.Invoke(new TextMessage(key, timeStamp, request, stream));
    }
}
