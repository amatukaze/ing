using System;
using System.IO;

namespace Sakuno.KanColle.Amatsukaze.Services
{
    public sealed class TextMessage
    {
        public TextMessage(string key, DateTimeOffset timeStamp, Stream stream)
        {
            Key = key;
            TimeStamp = timeStamp;
            Stream = stream;
        }

        public string Key { get; }
        public DateTimeOffset TimeStamp { get; }
        public Stream Stream { get; }
    }
}
