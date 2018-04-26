using System;
using System.IO;

namespace Sakuno.ING.Services
{
    public sealed class TextMessage
    {
        public TextMessage(string key, DateTimeOffset timeStamp, string request, Stream stream)
        {
            Key = key;
            TimeStamp = timeStamp;
            Request = request;
            Stream = stream;
        }

        public string Key { get; }
        public DateTimeOffset TimeStamp { get; }
        public string Request { get; }
        public Stream Stream { get; }
    }
}
